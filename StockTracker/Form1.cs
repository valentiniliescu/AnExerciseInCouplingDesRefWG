﻿using System;
using System.Windows.Forms;
// ReSharper disable SpecifyACultureInStringConversionExplicitly
// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace StockTracker
{
    public partial class Form1 : Form, E_IStockDisplayTable
    {
        private readonly StocksStore _stocksRepository;
        private readonly StockCollection _stockCollection;
        private readonly GainModel _gainModel;

        public Form1(StocksStore stocksStore, GainModel gainModel)
        {
            InitializeComponent();

            _stocksRepository = stocksStore;
            _gainModel = gainModel;
            _stockCollection = new StockCollection(stocksStore.LoadStocks());
            _stockCollection.Changed += (sender, e) => V_StockProcessor.RefreshTable(_stockCollection, _gainModel, _listViewStocks);
            _stockCollection.Changed += (sender, e) => SaveStocks();

            V_StockProcessor.RefreshTable(_stockCollection, _gainModel, _listViewStocks);
        }

        private void RefreshValues(object sender, EventArgs e)
        {
            E_StockProcessor.RefreshTable(_stockCollection, _gainModel, this);
            V_StockProcessor.RefreshTable(_stockCollection, _gainModel, _listViewStocks);
        }

        private void AddTicker(object sender, EventArgs e)
        {
            string ticker = _textBoxTicker.Text.TrimEnd('\n').ToUpper();
            double shares = Double.Parse(_textBoxShares.Text);
            double purchasePrice = Double.Parse(_textBoxPurchasePrice.Text);
            string purchaseDate = _textBoxPurchaseDate.Text;

            _stockCollection.Add(ticker, shares, purchasePrice, purchaseDate);
            _textBoxTicker.Text = String.Empty;
            _textBoxShares.Text = String.Empty;
            _textBoxPurchaseDate.Text = String.Empty;
            _textBoxPurchasePrice.Text = String.Empty;
        }

        private void SaveStocks()
        {
            _stocksRepository.SaveStocks(_stockCollection.EnumerateStocks());
        }

        private void DeleteStock(object sender, EventArgs e)
        {
            int index = _listViewStocks.SelectedIndices[0];
            if (index != -1)
            {
                _stockCollection.RemoveAt(index);
            }
        }

        private void ClearAllData(object sender, EventArgs e)
        {
            _stockCollection.RemoveAll();
        }

        public void AddItemToList(params object[] parameters)
        {
            var listViewItem = new ListViewItem(parameters[0].ToString());

            for (int i = 1; i < new[] {parameters}.Length; i++)
            {
                listViewItem.SubItems.Add(new[] {parameters}[i].ToString());
            }
            _listViewStocks.Items.Add(listViewItem);
        }

        public void ClearList()
        {
            _listViewStocks.Items.Clear();
        }
    }
}
