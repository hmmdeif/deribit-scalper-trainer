using Deribit_Scalper_Trainer.ContractModels;
using System;
using System.ComponentModel;

namespace Deribit_Scalper_Trainer.Controls.Models
{
    public class Position : INotifyPropertyChanged
    {
        public Position(int amount, decimal price, PositionEnum type, ref IContractModel instrument)
        {
            Amount = amount;
            Price = price;
            Type = type;
            Instrument = instrument;
            OpenDate = DateTime.UtcNow;
            UnrealisedPnl = 0M;
        }

        private int _amount = 0;
        public int Amount { 
            get
            {
                return _amount;
            }
            set
            {
                if (value != _amount)
                {
                    _amount = value;
                    NotifyPropertyChanged("Amount");
                }
            }
        }

        private decimal _price = 0M;
        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (value != _price)
                {
                    _price = value;
                    NotifyPropertyChanged("Price");
                }
            }
        }

        private decimal _unrealisedPnl = 0M;
        public decimal UnrealisedPnl
        {
            get
            {
                return _unrealisedPnl;
            }
            set
            {
                if (value != _unrealisedPnl)
                {
                    _unrealisedPnl = value;
                    NotifyPropertyChanged("UnrealisedPnl");
                }
            }
        }

        private decimal _liquidationPrice = 0M;
        public decimal LiquidationPrice
        {
            get
            {
                return _liquidationPrice;
            }
            set
            {
                if (value != _liquidationPrice)
                {
                    _liquidationPrice = value;
                    NotifyPropertyChanged("LiquidationPrice");
                }
            }
        }

        public PositionEnum Type { get; set; }
        public IContractModel Instrument { get; set; }
        public DateTime OpenDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
