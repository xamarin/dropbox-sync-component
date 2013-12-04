using System;
using DropBoxSync.iOS;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace MonkeyBox
{
	public class DropboxDatabase
	{
		public event EventHandler MonkeysUpdated;

		public Monkey[] Monkeys { get; set; }

		static DropboxDatabase shared;
		
		NSTimer timer;

		public bool AutoUpdating { get; set; }

		public static DropboxDatabase Shared {
			get {
				if (shared == null)
					shared = new DropboxDatabase ();
				return shared;
			}
		}

		DBDatastore store;

		public DropboxDatabase ()
		{
			Monkeys = new Monkey[0];
		}

		public void Init ()
		{
			if (store != null)
				return;
			DBError error;
			store = DBDatastore.OpenDefaultStoreForAccount (DBAccountManager.SharedManager.LinkedAccount, out error);
			var sync = store.Sync (out error);
            store.AddObserver (store, () => {
				LoadData ();
			});
			AutoUpdating = true;
			store.BeginInvokeOnMainThread(()=>{
				timer = NSTimer.CreateRepeatingScheduledTimer(1,()=>{
					if(!AutoUpdating)
						return;
					store.Sync (out error);
				});
			});

		}

		public Dictionary<string,DBRecord> records = new Dictionary<string, DBRecord> ();
		public Dictionary<string,Monkey> monkeyDictionary = new Dictionary<string, Monkey> ();

		public Task LoadData ()
		{
			var task = Task.Factory.StartNew (() => {
				var table = store.GetTable ("monkeys");
				DBError error = new DBError ();
				var results = table.Query (null, out error);
				
				if (results.Length == 0) {
					populateMonkeys ();
					return;
				}
				ProccessResults (results);
			});
			return task;
		}

		void ProccessResults (DBRecord[] results)
		{
			records = results.ToDictionary (x => x.Fields ["Name"].ToString (), x => x);
			foreach (var result in results) {
				var name = result.Fields ["Name"].ToString ();
				Monkey monkey;
				monkeyDictionary.TryGetValue (name, out monkey);
				if (monkey == null) {
					monkey = result.ToMonkey ();
					monkeyDictionary.Add (name, monkey);
				} else {
					monkey.Update (result);
				}
			}
			Monkeys = monkeyDictionary.Select (x => x.Value).OrderBy (x => x.Z).ToArray ();
			store.BeginInvokeOnMainThread (() => {
				if (MonkeysUpdated != null)
					MonkeysUpdated (this, EventArgs.Empty);
			});
		}

		public void DeleteAll ()
		{
			populated = false;
			var table = store.GetTable ("monkeys");
			DBError error;
			var results = table.Query (new NSDictionary (), out error);
			foreach (var result in results) {
				result.DeleteRecord ();
			}
			store.Sync (out error);
		}

		bool populated = false;

		void populateMonkeys ()
		{
			if (populated)
				return;
			populated = true;
			Monkeys = Monkey.GetAllMonkeys ();
			DBError error;
			var table = store.GetTable ("monkeys");
			foreach (var monkey in Monkeys) {
				bool inserted = false;
				table.GetOrInsertRecord (monkey.Name, monkey.ToDictionary (), inserted, out error);
			}
			store.Sync (out error);

		}

		public void Update (Monkey monkey)
		{
			DBRecord record;
			records.TryGetValue (monkey.Name, out record);
			record.Update (monkey.ToDictionary ());
			store.SyncAsync ();
		}

		public void Update()
		{
			store.SyncAsync ();
		}

		public void Reset()
		{
			DeleteAll();
		}
	}

	public static class DropboxHelper
	{
		public static NSDictionary ToDictionary (this Monkey monkey)
		{
			var keys = new NSString[] {
				new NSString("Name"),
				new NSString("Rotation"),
				new NSString("Scale"),
				new NSString ("X"),
				new NSString ("Y"),
				new NSString ("Z"),
			};
			var values = new NSString[] {
				new NSString(monkey.Name),
				new NSString(monkey.Rotation.ToString()),
				new NSString(monkey.Scale.ToString()),
				new NSString(monkey.X.ToString()),
				new NSString(monkey.Y.ToString()),
				new NSString(monkey.Z.ToString()),
			};
			return NSDictionary.FromObjectsAndKeys (values, keys);
		}

		public static Monkey ToMonkey (this DBRecord record)
		{
			return new Monkey ().Update (record);
			
		}

		public static Monkey Update (this Monkey monkey, DBRecord record)
		{
            monkey.Name = record.Fields ["Name"].ToString ();
            monkey.Rotation = ((NSNumber)record.Fields ["Rotation"]).FloatValue;
            monkey.Scale = ((NSNumber)record.Fields ["Scale"]).FloatValue;
            monkey.X = ((NSNumber)record.Fields ["X"]).FloatValue;
            monkey.Y =((NSNumber)record.Fields ["Y"]).FloatValue;
            monkey.Z =((NSNumber)record.Fields ["Z"]).IntValue;
			return monkey;
		}
	}
}

