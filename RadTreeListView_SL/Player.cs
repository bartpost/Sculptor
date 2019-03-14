using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Telerik.Windows.Data;

namespace RadTreeListView_SL
{
	/// <summary>
	/// A football player.
	/// </summary>
	public class Player : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string name;
		private string country;
		private int number;
		public int Number
		{
			get
			{
				return this.number;
			}
			set
			{
				if (this.number != value)
				{
					this.number = value;
					this.OnPropertyChanged("Number");
				}
			}
		}
		

		public string Name
		{
			get { return this.name; }
			set
			{
				if (value != this.name)
				{
					this.name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}


		public string Country
		{
			get { return this.country; }
			set
			{
				if (value != this.country)
				{
					this.country = value;
					this.OnPropertyChanged("Country");
				}
			}
		}

		public Player()
		{

		}

		public Player(string name, string country, int number)
		{
			this.name = name;
			this.country = country;
			this.number = number;
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, args);
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			return this.Name;
		}

		public static RadObservableCollection<Player> GetPlayers()
		{
			return new RadObservableCollection<Player>(Club.GetClubs().SelectMany(c => c.Players));
		}
	}
}