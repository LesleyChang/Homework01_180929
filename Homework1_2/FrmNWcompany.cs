using Homework1_2.Properties;
using NWModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homework1_2
{
    public partial class FrmNWcompany : Form
    {
        public FrmNWcompany()
        {
            InitializeComponent();
        }

        NorthwindEntities dbContext = new NorthwindEntities(GetConnectionString());

        private static string GetConnectionString()
        {
            EntityConnectionStringBuilder csBuilder = new EntityConnectionStringBuilder();
            csBuilder.Provider = "System.Data.SqlClient";
            csBuilder.ProviderConnectionString = Settings.Default.MyNotebookNWConnectionString;
            csBuilder.Metadata = string.Format
                ("res://{0}/{1}.csdl|res://{0}/{1}.ssdl|res://{0}/{1}.msl", typeof(NorthwindEntities).Assembly.FullName, "NorthwindModel");
            return csBuilder.ToString();
        }
        public string OrderBy { get; set; }
        public string Group { get; set; }
        public string Arrange { get; set; }
        public event EventHandler Order_DDBtnMenuItemCheckedChange;
        public event EventHandler Arrange_DDBtnMenuItemCheckedChange;
        public event EventHandler Group_DDBtnMenuItemCheckedChange;

        private void FrmNWcompany_Load(object sender, EventArgs e)
        {
            LoadLinkLabels();
            LoadListView1Columns();
            Add_NameTextBox_AutoCompleteCustomSourceItems();
            RegisterToolstripEvent();

        }


        private void LoadListView1Columns()
        {
            this.listView1.View = View.Details;
            this.listView1.Columns.Add("ProductID");
            this.listView1.Columns.Add("ProductName                  ");
            this.listView1.Columns.Add("CategoryName        ");
            this.listView1.Columns.Add("UnitsInStock");
            this.listView1.Columns.Add("UnitPrice");
            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        #region ToolStrip1(Search Method)

        private void Add_NameTextBox_AutoCompleteCustomSourceItems()
        {
            var q = dbContext.Products.Select(p => p.ProductName);
            foreach (var productName in q)
            {
                Name_TextBox.AutoCompleteCustomSource.Add(productName);
            }
        }
        private void Search_Btn_Click(object sender, EventArgs e)
        {
            string keyWord = this.Name_TextBox.Text.ToLower();
            decimal minPrice;
            decimal maxPrice;
            if (!decimal.TryParse(MinPrice_TextBox.Text, out minPrice))
            {
                MessageBox.Show("Please re-type the Min-Price", "Not Reasonable Price");
                this.MinPrice_TextBox.Text = "0";
                this.MinPrice_TextBox.Focus();
                return;
            }
            if (!decimal.TryParse(MaxPrice_TextBox.Text, out maxPrice))
            {
                MessageBox.Show("Please re-type the Max-Price", "Not Reasonable Price");
                this.MaxPrice_TextBox.Text = "500";
                this.MaxPrice_TextBox.Focus();
                return;
            }
            if (maxPrice < minPrice)
            {
                string tempText = MinPrice_TextBox.Text;
                MinPrice_TextBox.Text = MaxPrice_TextBox.Text;
                MaxPrice_TextBox.Text = tempText;
                decimal tempPrice = minPrice;
                minPrice = maxPrice;
                maxPrice = minPrice;
            }
            var q = dbContext.Products.Where(p => p.ProductName.ToLower().Contains(keyWord)
                && p.UnitPrice >= minPrice && p.UnitPrice <= maxPrice)
                .Select(p => new { p.ProductID, p.ProductName, p.Categories.CategoryName, p.UnitsInStock, p.UnitPrice });
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = q.ToList();
        }
        #endregion

        #region LinkLabel(load + ClickEvent)

        private void LoadLinkLabels()
        {
            this.splitContainer3.Panel2.Controls.Clear();
            Point location = new Point(12, 8);

            for (int i = 65; i <= 90; i++)
            {
                LinkLabel label = new LinkLabel();
                label.Name = ((char)i).ToString();
                label.Text = ((char)i).ToString();
                label.AutoSize = true;
                label.LinkColor = Color.Navy;
                label.Location = location;
                label.Font = new Font("Times New Roman", 10.2F);
                label.BringToFront();
                label.Click += CapitalLinkLabel_Click;
                location.X += 35;
                this.splitContainer3.Panel2.Controls.Add(label);
            }
        }

        private void CapitalLinkLabel_Click(object sender, EventArgs e)
        {
            LinkLabel label = sender as LinkLabel;
            var q = dbContext.Products.Where(p => p.ProductName.ToUpper().StartsWith(label.Text))
                .Select(p => new { p.ProductID, p.ProductName, p.Categories.CategoryName, p.UnitsInStock, p.UnitPrice });
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = q.ToList();
            this.treeView1.Nodes.Clear();
            this.listView1.Items.Clear();
        }

        #endregion

        #region ToolStrip1(stripToolClickEvent + stripMenuItem_ClickEvent)

        private void AllProduct_Btn_Click(object sender, EventArgs e)
        {
            var q = dbContext.Products
                .Select(p => new { p.ProductID, p.ProductName, p.Categories.CategoryName, p.UnitsInStock, p.UnitPrice });
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = q.ToList();
        }
        private void TreeView_Btn_Click(object sender, EventArgs e)
        {
            IQueryable<Products> query = dbContext.Products;
            this.treeView1.Nodes.Clear();
            
            var q1 = dbContext.Products.AsEnumerable();
            if (Arrange.Contains("Ascending"))
            {
                switch (OrderBy)
                {
                    case "ProductID":
                        q1 = q1.OrderBy(p => p.ProductID);
                        break;
                    case "ProductName":
                        q1 = q1.OrderBy(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        q1 = q1.OrderBy(p => p.UnitPrice);
                        break;
                }
                if (Group.Contains("CategoryName"))
                {
                    var q = q1.GroupBy(p => new { CategoryName = p.Categories.CategoryName })
                        .Select(p => new
                        {
                            CategoryName = $"{p.Key.CategoryName}({p.Count()})",
                            Count = p.Count(),
                            MinUnitPrice = p.Min(up => up.UnitPrice),
                            MaxUnitPrice = p.Max(up => up.UnitPrice),
                            group = p
                        });
                    this.dataGridView1.DataSource = q.ToList();
                    foreach (var group in q)
                    {
                        TreeNode x = this.treeView1.Nodes.Add(group.CategoryName.ToString());
                        foreach (var item in group.group)
                        {
                            x.Nodes.Add($"{item.ProductName} - ${item.UnitPrice}");
                        }
                    }
                    return;
                }
                if (Group.Contains("UnitPrice"))
                {
                    var q = dbContext.Products.OrderBy(p => p.ProductName).AsEnumerable()
                        .GroupBy(p => new { priceGroup = p.UnitPrice.GroupByUniPrice() })
                        .Select(p => new
                        {
                            PriceGroup = $"{p.Key.priceGroup}({p.Count()})",
                            Count = p.Count(),
                            MinUnitPrice = p.Min(up => up.UnitPrice),
                            MaxUnitPrice = p.Max(up => up.UnitPrice),
                            group = p
                        });
                    this.dataGridView1.DataSource = q.ToList();
                    foreach (var group in q)
                    {
                        TreeNode x = this.treeView1.Nodes.Add(group.PriceGroup.ToString());
                        foreach (var item in group.group)
                        {
                            x.Nodes.Add($"{item.ProductName} - ${item.UnitPrice}");
                        }
                    }
                    return;
                }
            }
            else
            {
                switch (OrderBy)
                {
                    case "ProductID":
                        q1 = q1.OrderByDescending(p => p.ProductID);
                        break;
                    case "ProductName":
                        q1 = q1.OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        q1 = q1.OrderByDescending(p => p.UnitPrice);
                        break;
                }
                if (Group.Contains("CategoryName"))
                {
                    var q = q1.GroupBy(p => new { CategoryName = p.Categories.CategoryName })
                        .Select(p => new
                        {
                            CategoryName = $"{p.Key.CategoryName}({p.Count()})",
                            Count = p.Count(),
                            MinUnitPrice = p.Min(up => up.UnitPrice),
                            MaxUnitPrice = p.Max(up => up.UnitPrice),
                            group = p
                        });
                    this.dataGridView1.DataSource = q.ToList();
                    foreach (var group in q)
                    {
                        TreeNode x = this.treeView1.Nodes.Add(group.CategoryName.ToString());
                        foreach (var item in group.group)
                        {
                            x.Nodes.Add($"{item.ProductName} - ${item.UnitPrice}");
                        }
                    }
                    return;
                }
                if (Group.Contains("UnitPrice"))
                {
                    var q = dbContext.Products.OrderBy(p => p.ProductName).AsEnumerable()
                        .GroupBy(p => new { priceGroup = p.UnitPrice.GroupByUniPrice() })
                        .Select(p => new
                        {
                            PriceGroup = $"{p.Key.priceGroup}({p.Count()})",
                            Count = p.Count(),
                            MinUnitPrice = p.Min(up => up.UnitPrice),
                            MaxUnitPrice = p.Max(up => up.UnitPrice),
                            group = p
                        });
                    this.dataGridView1.DataSource = q.ToList();
                    foreach (var group in q)
                    {
                        TreeNode x = this.treeView1.Nodes.Add(group.PriceGroup.ToString());
                        foreach (var item in group.group)
                        {
                            x.Nodes.Add($"{item.ProductName} - ${item.UnitPrice}");
                        }
                    }
                    return;
                }
            }
            

        }
        private void ListView_Btn_Click(object sender, EventArgs e)
        {
            IQueryable<Products> query = dbContext.Products;
            this.listView1.Items.Clear();

            var q1 = dbContext.Products.AsEnumerable();
            if (Arrange.Contains("Ascending"))
            {
                switch (OrderBy)
                {
                    case "ProductID":
                        q1 = q1.OrderBy(p => p.ProductID);
                        break;
                    case "ProductName":
                        q1 = q1.OrderBy(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        q1 = q1.OrderBy(p => p.UnitPrice);
                        break;
                }
                if (Group.Contains("CategoryName"))
                {
                    var q = q1.GroupBy(p => new { CategoryName = p.Categories.CategoryName })
                       .Select(p => new
                       {
                           CategoryName = $"{p.Key.CategoryName}({p.Count()})",
                           Count = p.Count(),
                           MinUnitPrice = p.Min(up => up.UnitPrice),
                           MaxUnitPrice = p.Max(up => up.UnitPrice),
                           group = p
                       });
                    foreach (var group in q)
                    {
                        ListViewGroup g = this.listView1.Groups.Add
                                     (group.CategoryName.ToString(), group.CategoryName.ToString());
                        foreach (var item in group.group)
                        {
                            ListViewItem viewItem = this.listView1.Items.Add(item.ProductID.ToString());
                            viewItem.Group = g;
                            viewItem.SubItems.Add(item.ProductName);
                            viewItem.SubItems.Add(item.Categories.CategoryName);
                            viewItem.SubItems.Add(item.UnitsInStock.ToString());
                            viewItem.SubItems.Add(item.UnitPrice.ToString());
                        }
                    }
                    return;
                }
                if (Group.Contains("UnitPrice"))
                {
                    var q = q1.OrderBy(p => p.ProductID).AsEnumerable()
                                .GroupBy(p => new { priceGroup = p.UnitPrice.GroupByUniPrice() })
                                .Select(p => new
                                {
                                    PriceGroup = $"{p.Key.priceGroup}({p.Count()})",
                                    Count = p.Count(),
                                    MinUnitPrice = p.Min(up => up.UnitPrice),
                                    MaxUnitPrice = p.Max(up => up.UnitPrice),
                                    group = p
                                });
                    foreach (var group in q)
                    {
                        ListViewGroup g = this.listView1.Groups.Add
                            (group.PriceGroup.ToString(), group.PriceGroup.ToString());
                        foreach (var item in group.group)
                        {
                            ListViewItem viewItem = this.listView1.Items.Add(item.ProductID.ToString());
                            viewItem.Group = g;
                            viewItem.SubItems.Add(item.ProductName);
                            viewItem.SubItems.Add(item.Categories.CategoryName);
                            viewItem.SubItems.Add(item.UnitsInStock.ToString());
                            viewItem.SubItems.Add(item.UnitPrice.ToString());

                        }
                    }
                    return;
                }

            }
            else
            {
                switch (OrderBy)
                {
                    case "ProductID":
                        q1 = q1.OrderByDescending(p => p.ProductID);
                        break;
                    case "ProductName":
                        q1 = q1.OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        q1 = q1.OrderByDescending(p => p.UnitPrice);
                        break;
                }
                if (Group.Contains("CategoryName"))
                {
                    var q = q1.GroupBy(p => new { CategoryName = p.Categories.CategoryName })
                       .Select(p => new
                       {
                           CategoryName = $"{p.Key.CategoryName}({p.Count()})",
                           Count = p.Count(),
                           MinUnitPrice = p.Min(up => up.UnitPrice),
                           MaxUnitPrice = p.Max(up => up.UnitPrice),
                           group = p
                       });
                    foreach (var group in q)
                    {
                        ListViewGroup g = this.listView1.Groups.Add
                                     (group.CategoryName.ToString(), group.CategoryName.ToString());
                        foreach (var item in group.group)
                        {
                            ListViewItem viewItem = this.listView1.Items.Add(item.ProductID.ToString());
                            viewItem.Group = g;
                            viewItem.SubItems.Add(item.ProductName);
                            viewItem.SubItems.Add(item.Categories.CategoryName);
                            viewItem.SubItems.Add(item.UnitsInStock.ToString());
                            viewItem.SubItems.Add(item.UnitPrice.ToString());
                        }
                    }
                    return;
                }
                if (Group.Contains("UnitPrice"))
                {
                    var q = q1.OrderBy(p => p.ProductID).AsEnumerable()
                                .GroupBy(p => new { priceGroup = p.UnitPrice.GroupByUniPrice() })
                                .Select(p => new
                                {
                                    PriceGroup = $"{p.Key.priceGroup}({p.Count()})",
                                    Count = p.Count(),
                                    MinUnitPrice = p.Min(up => up.UnitPrice),
                                    MaxUnitPrice = p.Max(up => up.UnitPrice),
                                    group = p
                                });
                    foreach (var group in q)
                    {
                        ListViewGroup g = this.listView1.Groups.Add
                            (group.PriceGroup.ToString(), group.PriceGroup.ToString());
                        foreach (var item in group.group)
                        {
                            ListViewItem viewItem = this.listView1.Items.Add(item.ProductID.ToString());
                            viewItem.Group = g;
                            viewItem.SubItems.Add(item.ProductName);
                            viewItem.SubItems.Add(item.Categories.CategoryName);
                            viewItem.SubItems.Add(item.UnitsInStock.ToString());
                            viewItem.SubItems.Add(item.UnitPrice.ToString());

                        }
                    }
                    return;
                }
            }

        }


        #region ToolStripMenu(下拉選單)
        private void RegisterToolstripEvent()
        {
            this.OrderBy = "ProductID"; //Initialized
            this.Arrange = "Ascending";
            this.Group = "CategoryName";

            this.Order_DDBtnMenuItemCheckedChange += FrmNWcompany_Order_DDBtnMenuItemCheckedChange;
            this.Arrange_DDBtnMenuItemCheckedChange += FrmNWcompany_Arrange_DDBtnMenuItemCheckedChange;
            this.Group_DDBtnMenuItemCheckedChange += FrmNWcompany_Group_DDBtnMenuItemCheckedChange;

            foreach (var item in this.Order_DDBtn.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Click += (object sender, EventArgs e) =>
                {
                    if (Order_DDBtnMenuItemCheckedChange != null)
                    {
                        Order_DDBtnMenuItemCheckedChange.Invoke(sender, e);
                    }
                };
            }
            foreach (var item in this.Arrange_DDBtn.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Click += (object sender, EventArgs e) =>
                {
                    if (Arrange_DDBtnMenuItemCheckedChange != null)
                    {
                        Arrange_DDBtnMenuItemCheckedChange.Invoke(sender, e);
                    }
                };
            }
            foreach (var item in this.Group_DDBtn.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Click += (object sender, EventArgs e) =>
                {
                    if (Group_DDBtnMenuItemCheckedChange != null)
                    {
                        Group_DDBtnMenuItemCheckedChange.Invoke(sender, e);
                    }
                };
            }
        }

        private void FrmNWcompany_Group_DDBtnMenuItemCheckedChange(object sender, EventArgs e)
        {
            foreach (var item in this.Group_DDBtn.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Checked = false;
            }
            ToolStripMenuItem itemClicked = sender as ToolStripMenuItem;
            itemClicked.Checked = true;
            this.Group = itemClicked.Tag.ToString();
            this.ListView_Btn_Click(sender, e);
            this.TreeView_Btn_Click(sender, e);

        }
        private void FrmNWcompany_Arrange_DDBtnMenuItemCheckedChange(object sender, EventArgs e)
        {
            foreach (var item in this.Arrange_DDBtn.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Checked = false;
            }
            ToolStripMenuItem itemClicked = sender as ToolStripMenuItem;
            itemClicked.Checked = true;
            this.Arrange = itemClicked.Name.ToString();
            this.ListView_Btn_Click(sender, e);
            this.TreeView_Btn_Click(sender, e);
        }
        private void FrmNWcompany_Order_DDBtnMenuItemCheckedChange(object sender, EventArgs e)
        {
            foreach (var item in this.Order_DDBtn.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Checked = false;
            }
            ToolStripMenuItem itemClicked = sender as ToolStripMenuItem;
            itemClicked.Checked = true;
            this.OrderBy = itemClicked.Tag.ToString();
            this.ListView_Btn_Click(sender, e);
            this.TreeView_Btn_Click(sender, e);
        }



        #endregion

        #endregion


    }

    public static class IQueryableExtension
    {
        private static MethodInfo orderbyAscInfo = null;
        private static MethodInfo orderbyDecInfo = null;
        private static MethodInfo groupbyInfo = null;
        public static IQueryable<T> OrderByAsc<T>(this IQueryable<T> query, string property) where T : class
        {
            Type entityType = typeof(T);
            Type entityPropertyType = entityType.GetProperty(property).PropertyType;
            var orderPara = Expression.Parameter(entityType, "o");
            var orderExpr = Expression.Lambda(Expression.Property(orderPara, property), orderPara);
            if (orderbyAscInfo == null)
            {
                orderbyAscInfo = typeof(Queryable).GetMethods().Single(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
            }

            return orderbyAscInfo.MakeGenericMethod(new Type[] { entityType, entityPropertyType }).Invoke(null, new object[] { query, orderExpr }) as IQueryable<T>;

        }
        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string property) where T : class
        {
            Type entityType = typeof(T);
            Type entityPropertyType = entityType.GetProperty(property).PropertyType;

            var orderPara = Expression.Parameter(entityType, "o");
            var orderExpr = Expression.Lambda(Expression.Property(orderPara, property), orderPara);

            if (orderbyDecInfo == null)
            {
                orderbyDecInfo = typeof(Queryable).GetMethods().Single(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);
            }

            return orderbyDecInfo.MakeGenericMethod(new Type[] { entityType, entityPropertyType })
                .Invoke(null, new object[] { query, orderExpr }) as IQueryable<T>;
        }
        public static IQueryable<T> GroupByCheckedItem<T>(this IQueryable<T> query, string property) where T : class
        {
            Type entityType = typeof(T);
            Type entityPropertyType = entityType.GetProperty(property).PropertyType;

            var groupPara = Expression.Parameter(entityType, "g");
            var groupExpr = Expression.Lambda(Expression.Property(groupPara, property), groupPara);

            if(groupbyInfo == null)
            {
                groupbyInfo = typeof(IQueryable).GetMethods().Single(x => x.Name == "GroupBy" && x.GetParameters().Length == 2);
            }
            return groupbyInfo.MakeGenericMethod(new Type[] { entityType, entityPropertyType })
                .Invoke(null, new object[] { query, groupExpr }) as IQueryable<T>;
        }

    }
}
