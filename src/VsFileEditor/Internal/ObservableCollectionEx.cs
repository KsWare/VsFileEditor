using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.VsFileEditor.Internal;

public class ObservableCollectionEx<T> : ObservableCollection<T>, ISupportInitialize {

	private int _suppressNotification;
	
	public void BeginInit() {
		_suppressNotification++;
	}
	
	public void EndInit() {
		_suppressNotification--;
		if(_suppressNotification>0) return;
		NotifyReset();
		_suppressNotification = 0;
	}


	public void ReplaceAll(IEnumerable<T> items) {
		BeginInit();
		Clear();
		foreach (var item in items) Add(item);
		EndInit();
	}


	public void AddRange(IEnumerable<T> newItems, bool bulk = false) {
		if (bulk) BeginInit();
		foreach (var item in newItems) Add(item);
		if (bulk) EndInit();
	}

	public void NotifyReset() {
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	}

	protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
		if (_suppressNotification==0) base.OnCollectionChanged(e);
	}

	
}
