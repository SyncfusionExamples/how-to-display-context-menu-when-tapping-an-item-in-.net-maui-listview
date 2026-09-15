# How to show context menu for items in .NET MAUI ListView (SfListView)?

The [.NET MAUI ListView](https://www.syncfusion.com/maui-controls/maui-listView) allows displaying a [.NET MAUI Popup (SfPopup)](https://help.syncfusion.com/maui/popup/getting-started) as a context menu with different menu items when an item is long pressed. This is achieved by customizing the ListView and using a custom view. The popup menu is displayed based on the touch position using the [Position](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.ListView.ItemLongPressEventArgs.html#Syncfusion_Maui_ListView_ItemLongPressEventArgs_Position) property from the [ItemLongPress](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.ListView.SfListView.html#Syncfusion_Maui_ListView_SfListView_ItemLongPress) event.

**Defining ListView:**
 
 ```
  <ContentPage xmlns:syncfusion="clr-namespace:Syncfusion.Maui.ListView;assembly=Syncfusion.Maui.ListView">
    <ContentPage.BindingContext>
        <local:ContactsViewModel x:Name="viewModel"/>
    </ContentPage.BindingContext>
    <ContentPage.Content>
            <Grid >
            <syncfusion:SfListView x:Name="listView" ItemsSource="{Binding contactsinfo}">
                <syncfusion:SfListView.Behaviors>
                    <local:Behavior/>
                </syncfusion:SfListView.Behaviors>
                <syncfusion:SfListView.ItemTemplate>
                        <DataTemplate>
                             <StackLayout>
                                    <Grid VerticalOptions="Center">
                                        <Image Source="{Binding ContactImage}" Grid.Column="0"/>
                                        <StackLayout Grid.Column="1">
                                            <Label Text="{Binding ContactName}"/>
                                            <Label Text="{Binding ContactNumber}"/>
                                        </StackLayout>
                                    </Grid>
                                    <StackLayout HeightRequest="1" BackgroundColor="Gray"/>
                                </StackLayout>
                          </DataTemplate>
                    </syncfusion:SfListView.ItemTemplate>
                </syncfusion:SfListView>
            </Grid>
    </ContentPage.Content>
</ContentPage>
 ```
 

**Defining Popup with Sort, Delete, and Cancel options:**
 
 ```
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
          ListView.ItemLongPress += ListView_ItemLongPress; 
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

      private void ListView_ItemLongPress(object sender, ItemLongPressEventArgs e)
      {
          item = e.DataItem as Contacts;
          var Layout = ListView.ItemsLayout as LinearLayout;
          var rowItems = Layout!.GetType().GetRuntimeFields().First(p => p.Name == "items").GetValue(Layout) as IList;

          foreach (ListViewItemInfo iteminfo in rowItems)
          {
              if(iteminfo.Element != null && iteminfo.DataItem == item)
              {
                  itemView = iteminfo.Element as View;
                  break;
              }
          }
          popupLayout = new SfPopup();
          popupLayout.HeightRequest = 150;
          popupLayout.WidthRequest = 100;
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
 ```
 
 
 ![Screenshot_2024-02-28_165338.png](https://support.syncfusion.com/kb/attachment/article/15412/inline?token=eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjE5MDU0Iiwib3JnaWQiOiIzIiwiaXNzIjoic3VwcG9ydC5zeW5jZnVzaW9uLmNvbSJ9.Bu1LzWikn-hqmxTa3xCNQZLNB7nd6a4cNUCboTzflZk)

### Info
Due to limitations in accurately obtaining the touch position relative to the selected item during the LongPress event, the popup is strategically positioned at the bottom right of the screen, ensuring a consistent and visually appealing user experience.


**Conclusion:**

I hope you enjoyed learning how to add a context menu for items in .NET MAUI ListView.

You can refer to our [.NET MAUI ListView](https://www.syncfusion.com/maui-controls/maui-listView) feature tour page to know about its other groundbreaking feature representations and [documentation](https://help.syncfusion.com/maui/listview/getting-started), and how to quickly get started with configuration specifications.

Check out our components from the [License and Downloads](https://www.syncfusion.com/sales/teamlicense) page for current customers. If you are new to Syncfusion®, try our 30-day [free trial](https://www.syncfusion.com/downloads/maui) to check out our other controls.

Please let us know in the comments section if you have any queries or require clarification. You can also contact us through our [support forums](https://www.syncfusion.com/forums/), [Direct-Trac](https://support.syncfusion.com/create), or [feedback portal](https://www.syncfusion.com/feedback/maui?control=sflistview). We are always happy to assist you!
