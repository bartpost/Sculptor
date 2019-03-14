using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Windows.Data;

namespace RadTreeListView_SL
{
	/// <summary>
	/// A football club.
	/// </summary>
	public class Club : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string name;
		private RadObservableCollection<Player> players;
		private string country;
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


		public RadObservableCollection<Player> Players
		{
			get
			{
				if (null == this.players)
				{
					this.players = new RadObservableCollection<Player>();
				}

				return this.players;
			}
		}

		public Club()
		{

		}

		public Club(string name, string country, int number)
		{
			this.name = name;
			this.country = country;
			this.number = number;
		}

		public Club(string name, RadObservableCollection<Player> players, string country, int number)
			: this(name, country, number)
		{
			this.players = players;
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

		public static RadObservableCollection<Club> GetClubs()
		{
			RadObservableCollection<Club> clubs = new RadObservableCollection<Club>();
			Club club;

			// Liverpool
			club = new Club("Liverpool", "England", 1);
			club.Players.Add(new Player("Pepe Reina", "Spain", 1));
			club.Players.Add(new Player("Jamie Carragher", "England", 7));
			club.Players.Add(new Player("Steven Gerrard", "England", 4));
			club.Players.Add(new Player("Fernando Torres", "Spain", 90));
			clubs.Add(club);

			// Manchester Utd.
			club = new Club("Manchester Utd.", "England",2);
			club.Players.Add(new Player("Edwin van der Sar", "Netherlands",2));
			club.Players.Add(new Player("Rio Ferdinand", "England",34));
			club.Players.Add(new Player("Ryan Giggs", "Wales",7));
			club.Players.Add(new Player("Wayne Rooney", "England", 9));
			clubs.Add(club);

			// Chelsea
			club = new Club("Chelsea", "England", 3);
			//club.Players.Add(new Player("Petr Čech", "Czech Republic",3));
			//club.Players.Add(new Player("John Terry", "England", 45));
			//club.Players.Add(new Player("Frank Lampard", "England", 45));
			//club.Players.Add(new Player("Nicolas Anelka", "France",23));
			clubs.Add(club);

			// Arsenal
			club = new Club("Arsenal", "England",4);
			club.Players.Add(new Player("Manuel Almunia", "Spain" ,48));
			club.Players.Add(new Player("Gaël Clichy", "France",21));
			club.Players.Add(new Player("Cesc Fàbregas", "Spain",22));
			club.Players.Add(new Player("Robin van Persie", "Netherlands", 23));
			clubs.Add(club);			
			return clubs;
		}
	}
}