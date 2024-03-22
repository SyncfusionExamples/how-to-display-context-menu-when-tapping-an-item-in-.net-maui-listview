using Syncfusion.Maui.DataSource;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.ListView.Helpers;
using Syncfusion.Maui.Popup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ItemTappedEventArgs = Syncfusion.Maui.ListView.ItemTappedEventArgs;
using ListSortDirection = Syncfusion.Maui.DataSource.ListSortDirection;


namespace ListViewMaui
{
    public class Behavior : Behavior<SfListView>
    {
        SfListView ListView;
        int sortorder = 0;
        Contacts item;
        View itemView;
        SfPopup popupLayout;
        protected override void OnAttachedTo(SfListView listView)
        {
            ListView = listView;
            ListView.ItemLongPress += ListView_ItemHolding; ;
            ListView.ScrollStateChanged += ListView_ScrollStateChanged;
            ListView.ItemTapped += ListView_ItemTapped;
            base.OnAttachedTo(listView);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (popupLayout != null)
            {
                popupLayout.Dismiss();
            }
        }

        private void ListView_ScrollStateChanged(object sender, ScrollStateChangedEventArgs e)
        {
            if (popupLayout != null)
            {
                popupLayout.Dismiss();
            }
        }

        private void ListView_ItemHolding(object sender, ItemLongPressEventArgs e)
        {
            item = e.DataItem as Contacts;
            var Layout = ListView.ItemsLayout as LinearLayout;
            var rowItems = Layout!.GetType().GetRuntimeFields().First(p => p.Name == "items").GetValue(Layout) as IList;

            foreach (ListViewItemInfo iteminfo in rowItems)
            {
                if(iteminfo.Element != null && iteminfo.DataItem == item)
                {
                    itemView = iteminfo.Element as View;
                }
            }
                popupLayout = new SfPopup();
            popupLayout.HeightRequest = 200;
            popupLayout.WidthRequest = 150;
            popupLayout.ContentTemplate = new DataTemplate(() =>
            {

                var mainStack = new StackLayout();
                mainStack.BackgroundColor = Colors.Teal;
              

                var deletedButton = new Button()
                {
                    Text = "Delete",
                    HeightRequest=50,
                    BorderWidth=1,
                    BorderColor = Colors.White,
                    BackgroundColor=Colors.Teal,
                    TextColor = Colors.White
                };
                deletedButton.Clicked += DeletedButton_Clicked;
                var AddButton = new Button()
                {
                    Text = "Add",
                    HeightRequest = 50,
                    BorderWidth = 1,
                    BorderColor = Colors.White,
                    BackgroundColor = Colors.Teal,
                    TextColor = Colors.White,
                    Command = (ListView.BindingContext as ContactsViewModel).AddCommand
                   
                };
                var Sortbutton = new Button()
                {
                    Text = "Sort",
                    BorderWidth = 1,
                    HeightRequest = 50,
                    BorderColor = Colors.White,
                    BackgroundColor = Colors.Teal,
                    TextColor = Colors.White,
                };
                Sortbutton.Clicked += Sortbutton_Clicked;
                var Dismiss = new Button()
                {
                    Text = "Cancel",
                    HeightRequest = 50,
                    BorderWidth = 1,
                    BorderColor = Colors.White,
                    BackgroundColor = Colors.Teal,
                    TextColor = Colors.White,
                };
                Dismiss.Clicked += Dismiss_Clicked;
                mainStack.Children.Add(deletedButton);
                mainStack.Children.Add(AddButton);
                mainStack.Children.Add(Sortbutton);
                mainStack.Children.Add(Dismiss);
                return mainStack;

            });

          
           
            popupLayout.PopupStyle.CornerRadius = 5;
            popupLayout.ShowHeader = false;
            popupLayout.ShowFooter = false;
            popupLayout.ShowRelativeToView(itemView, PopupRelativePosition.AlignBottomRight);

        }

        private void Dismiss_Clicked(object? sender, EventArgs e)
        {
            popupLayout.Dismiss();  
        }

        private void Sortbutton_Clicked(object? sender, EventArgs e)
        {
            if (ListView == null)
                return;

            ListView.DataSource.SortDescriptors.Clear();
            popupLayout.Dismiss();
            ListView.DataSource.LiveDataUpdateMode = LiveDataUpdateMode.AllowDataShaping;
            if (sortorder == 0)
            {
                ListView.DataSource.SortDescriptors.Add(new SortDescriptor { PropertyName = "ContactName", Direction = ListSortDirection.Descending });
                sortorder = 1;
            }
            else
            {
                ListView.DataSource.SortDescriptors.Add(new SortDescriptor { PropertyName = "ContactName", Direction = ListSortDirection.Ascending });
                sortorder = 0;
            }
        }

       
        private void DeletedButton_Clicked(object sender, EventArgs e)
        {
            
            if (ListView == null)
                return;

            var source = ListView.ItemsSource as IList;

            if (source != null && source.Contains(item))
            {
                source.Remove(item);
                App.Current.MainPage.DisplayAlert("Alert", item.ContactName + " is deleted", "OK");
            }
            else
                App.Current.MainPage.DisplayAlert("Alert", "Unable to delete the item", "OK");

            item = null;
            source = null;
        }
    }
}
