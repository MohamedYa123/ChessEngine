using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ChessBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lightcolor =Color.White ;// this.BackColor;
            setcolors();
        }
        public int numofrowsandcolumns = 8;
        public Color darkcolor = Color.Black;
        public Color lightcolor;
        public Panel[,] allpieces;
        public void setcolors()
        {
            int cc = 1;
            allpieces = new Panel[numofrowsandcolumns,numofrowsandcolumns];
            for(int i = 0; i < numofrowsandcolumns; i++)
            {
                cc++;
                for (int i2 = 0; i2 < numofrowsandcolumns; i2++)
                {
                    Panel p = new Panel();
                    if (cc%2!=0)//(i % 2 != 0 || i2%2!=0)
                    {
                        //color with dark ##
                        
                        p.Location = new Point(iboard.Top + i * iboard.Height / numofrowsandcolumns, iboard.Top + i2 * iboard.Width / numofrowsandcolumns);
                        p.Size = new Size(iboard.Width / numofrowsandcolumns, iboard.Height / numofrowsandcolumns);
                        p.BackColor = darkcolor;
                        p.SendToBack();
                        
                    }
                    else
                    {
                        p.Location = new Point(iboard.Top + i * iboard.Height / numofrowsandcolumns, iboard.Top + i2 * iboard.Width / numofrowsandcolumns);
                        p.Size = new Size(iboard.Width / numofrowsandcolumns, iboard.Height / numofrowsandcolumns);
                        p.BackColor = lightcolor;
                        p.SendToBack();
                        
                    }
                    //     p.Size = new Size(90, 90);
                    Button l = new Button();
                    //l.AutoSize = true;
                    l.Hide();
                    l.Location = new Point(p.Width/10, p.Width / 10);// p.Location;
                  //  l.Text = "kk";
                    Button b = new Button();
                    b.Location = new Point(p.Top+10,p.Left+10);
                    b.Text = "ck";
                    b.Size = new Size(30, 40);
                  //  p.Controls.Add(b);
                  //  this.Text = l.Location.X + " " + l.Location.Y;
                  //  this.Text = p.Location.X + " " + p.Location.Y;
                    p.Controls.Add(l);
                    allpieces[i, i2] = p;
                    cc++;
                    iboard.Controls.Add(p);
                }
            }
            iboard.Width *= 10/4;
            iboard.Height *= 10/4;
        }
        bool flip = false;
        private void button1_Click(object sender, EventArgs e)
        {
            board b = new board(32);
            
            b.putpieces();
            Thinker th = new Thinker();
         //   for (int i = 0; i < 4; i++)
            {
                //     th.calc(b, 0);
                situation stm = new situation();
                stm.sidePlayed = 1;
                stm.current = b;
                  th.runallthreads();
                th.runCollectors();
                th.calc(null,stm, 5, new counter(), 0, true, false);
               th.wait();
                th.stopthreads();
            }
            int count = 0;
            
            foreach(var d in allpieces)
            {
                int postx = d.Location.X / d.Width;
                int posty = d.Location.Y / d.Width;
                if (!flip)
                {
                    postx = 8 - postx;
                    posty = 8 - posty;
                }
                else
                {
                    postx += 1;
                    posty += 1;
                }
                d.Click += delegate { viewposition(b); };
                count++;
                foreach (var c in d.Controls)
                {
                    try
                    {
                        Button ll = (Button)c;
                        bool notdf = false;
                        foreach (var p in b.pieces)
                        {
                            if (p.x!=postx || p.y != posty)
                            {
                                continue;
                            }
                            notdf = true;
                            ll.Text = p.ToString();
                            ll.Size = new Size(d.Width*8/9, d.Width * 6 / 9);
                            //this.Text = ll.Location.X + " " + ll.Location.Y;
                            ll.BringToFront();
                            ll.Show();
                            ll.Name = "piece";
                            ll.BackColor = Color.Black;
                            ll.ForeColor = Color.Yellow;
                            if (p.side == 0)
                            {
                                ll.BackColor = Color.Wheat;
                                ll.ForeColor = Color.Black;
                            }
                            FieldInfo f1 = typeof(Control).GetField("EventClick",
                                BindingFlags.Static | BindingFlags.NonPublic);

                            /*object obj = f1.GetValue(ll);
                            PropertyInfo pi = ll.GetType().GetProperty("Events",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            EventHandlerList list = (EventHandlerList)pi.GetValue(ll, null);
                            list.RemoveHandler(obj, list[obj]);*/
                            ll.Click += delegate
                            {
                                viewposition(b);
                                getclicks( p,  postx,  posty,  ll,  b);
                            };
                          //  ll.ForeColor = Color.White;
                        }
                        if (!notdf)
                        {
                            ll.Hide();
                        }
                        /* Label l = new Label();
                         l.Location = d.Location;
                         l.AutoSize = true;
                         l.Text= b.pieces[count].ToString();
                         l.BringToFront();
                         l.Show();
                         l.ForeColor = Color.Yellow;
                         iboard.Controls.Add(l); */
                    }
                    catch
                    {

                    }
                }
                
            }
        }
        public void getclicks(Piece p,int postx,int posty,Button ll,board b)
        {
            b.resetsquares();
            b.putpiecesinsquares();
            b.getchecks();
            var m = p.getmove();
            //  foreach (var s in m)
            {


                foreach (var dd in allpieces)
                {
                    int postxx = dd.Location.X / dd.Width;
                    int postyy = dd.Location.Y / dd.Width;
                    if (!flip)
                    {
                        postxx = 8 - postxx;
                        postyy = 8 - postyy;
                    }
                    else
                    {
                        postxx += 1;
                        postyy += 1;
                    }
                    foreach (var cc in dd.Controls)
                    {
                        try
                        {
                            Button l = (Button)cc;
                           // bool notdff = false;
                            foreach (var s in m)
                            {
                                if (s.position1[0] != postxx || s.position1[1] != postyy)
                                {
                                    if (l.Name != postx + " " + posty && l.Name != "piece")
                                    {
                                        l.Hide();
                                    }
                                    continue;
                                }
                                l.Name = postx + " " + posty;
                                l.Show();
                                l.BackColor = Color.Green;
                                l.Text = "";
                                l.Size = ll.Size;
                                //   l.Click -= delegate { };
                                FieldInfo f1 = typeof(Control).GetField("EventClick",
                                BindingFlags.Static | BindingFlags.NonPublic);

                                object obj = f1.GetValue(l);
                                PropertyInfo pi = l.GetType().GetProperty("Events",
                                    BindingFlags.NonPublic | BindingFlags.Instance);
                                EventHandlerList list = (EventHandlerList)pi.GetValue(l, null);
                                list.RemoveHandler(obj, list[obj]);
                                l.Click += delegate
                                {
                                    viewposition(b);
                                    b.aply_move(s);
                                    if (s.position2 != null)
                                    {

                                        foreach (var dk in allpieces)
                                        {
                                            int postxxk = dk.Location.X / dk.Width;
                                            int postyyk = dk.Location.Y / dk.Width;
                                            if (!flip)
                                            {
                                                postxxk = 8 - postxxk;
                                                postyyk = 8 - postyyk;
                                            }
                                            else
                                            {
                                                postxxk += 1;
                                                postyyk += 1;
                                            }
                                            foreach (var cck in dk.Controls)
                                            {
                                                try
                                                {
                                                    Button lk = (Button)cck;
                                                    // bool notdff = false;
                                                    //foreach (var sk in m)
                                                    {
                                                        if (s.position2[0] == postxxk && s.position2[1] == postyyk)
                                                        {



                                                            FieldInfo f1f = typeof(Control).GetField("EventClick",
                                                            BindingFlags.Static | BindingFlags.NonPublic);

                                                            object obf = f1f.GetValue(lk);
                                                            PropertyInfo pf = lk.GetType().GetProperty("Events",
                                                                BindingFlags.NonPublic | BindingFlags.Instance);
                                                            EventHandlerList list1f = (EventHandlerList)pf.GetValue(lk, null);
                                                            list1f.RemoveHandler(obf, list1f[obf]);
                                                            foreach (var pp in b.pieces)
                                                            {
                                                                if (pp.x != postxxk || pp.y != postyyk)
                                                                {
                                                                    continue;
                                                                }
                                                               // notdf = true;
                                                                lk.Text = pp.ToString();
                                                                lk.Size = new Size(dd.Width * 8 / 9, dd.Width * 6 / 9);
                                                                this.Text = ll.Location.X + " " + ll.Location.Y;
                                                                lk.BringToFront();
                                                                lk.Show();
                                                                lk.Name = "piece";
                                                                lk.BackColor = Color.Black;
                                                                lk.ForeColor = Color.Yellow;
                                                                if (pp.side == 0)
                                                                {
                                                                    lk.BackColor = Color.Wheat;
                                                                    lk.ForeColor = Color.Black;
                                                                }
                                                                lk.Click += delegate
                                                                {
                                                                    viewposition(b);
                                                                    getclicks(pp, postxxk, postyyk, lk, b);
                                                                };
                                                                //  ll.ForeColor = Color.White;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch { }
                                             } 
                                        } 
                                     }
                                    
                                    l.Name = "piece";
                                    ll.Name = "";
                                    ll.Text = "";
                                    FieldInfo ff1 = typeof(Control).GetField("EventClick",
                                    BindingFlags.Static | BindingFlags.NonPublic);

                                    object objf = ff1.GetValue(l);
                                    PropertyInfo pif = l.GetType().GetProperty("Events",
                                        BindingFlags.NonPublic | BindingFlags.Instance);
                                    EventHandlerList listf = (EventHandlerList)pif.GetValue(l, null);
                                    listf.RemoveHandler(objf, listf[objf]);
                                    l.Click += delegate {
                                        viewposition(b);
                                        getclicks(p, p.x, p.y, l, b);
                                     };
                                    viewposition(b);
                                };
                            }
                        }
                        catch { }
                    }
                }


            }
           // b.getchecks();
            //   viewposition(b);
        }
        public void viewposition(board b)
        {
            foreach (var d in allpieces)
            {
                int postx = d.Location.X / d.Width;
                int posty = d.Location.Y / d.Width;
                if (!flip)
                {
                    postx = 8 - postx;
                    posty = 8 - posty;
                }
                else
                {
                    postx += 1;
                    posty += 1;
                }

                foreach (var c in d.Controls)
                {
                    try
                    {
                        Button ll = (Button)c;
                        bool notdf = false;
                        foreach (var p in b.pieces)
                        {
                            if (p.x != postx || p.y != posty)
                            {
                                continue;
                            }
                            notdf = true;
                            ll.Text = p.ToString();
                            ll.Size = new Size(d.Width * 8 / 9, d.Width * 6 / 9);
                            this.Text = ll.Location.X + " " + ll.Location.Y;
                            ll.BringToFront();
                            ll.Show();
                            ll.BackColor = Color.Black;
                            ll.ForeColor = Color.Yellow;
                            if (p.side == 0)
                            {
                                ll.BackColor = Color.Wheat;
                                ll.ForeColor = Color.Black;
                            }
                            FieldInfo f1 = typeof(Control).GetField("EventClick",
                            BindingFlags.Static | BindingFlags.NonPublic);

                            object obj = f1.GetValue(ll);
                            PropertyInfo pi = ll.GetType().GetProperty("Events",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            EventHandlerList list = (EventHandlerList)pi.GetValue(ll, null);
                            list.RemoveHandler(obj, list[obj]);
                            ll.Click += delegate
                            {
                                b.resetsquares();
                                b.putpiecesinsquares();
                                b.getchecks();
                                viewposition(b);
                                var m = p.getmove();
                                //  foreach (var s in m)
                                {


                                    foreach (var dd in allpieces)
                                    {
                                        int postxx = dd.Location.X / dd.Width;
                                        int postyy = dd.Location.Y / dd.Width;
                                        if (!flip)
                                        {
                                            postxx = 8 - postxx;
                                            postyy = 8 - postyy;
                                        }
                                        else
                                        {
                                            postxx += 1;
                                            postyy += 1;
                                        }
                                        foreach (var cc in dd.Controls)
                                        {
                                            try
                                            {
                                                Button l = (Button)cc;
                                    //            bool notdff = false;
                                                foreach (var s in m)
                                                {
                                                    if (s.position1[0] != postxx || s.position1[1] != postyy)
                                                    {
                                                        if (l.Text == "")
                                                        {
                                                            // l.Hide();
                                                        }
                                                        continue;
                                                    }

                                                    l.Show();
                                                    l.BackColor = Color.Green;
                                                    l.Text = "";
                                                    l.Size = ll.Size;

                                                    FieldInfo ff1 = typeof(Control).GetField("EventClick",
                                                    BindingFlags.Static | BindingFlags.NonPublic);

                                                    object objf = ff1.GetValue(l);
                                                    PropertyInfo pif = l.GetType().GetProperty("Events",
                                                        BindingFlags.NonPublic | BindingFlags.Instance);
                                                    EventHandlerList listf = (EventHandlerList)pif.GetValue(l, null);
                                                    listf.RemoveHandler(objf, listf[objf]);
                                                    l.Click += delegate
                                                    {
                                                        viewposition(b);
                                                        b.aply_move(s);
                                                        if (s.position2 != null)
                                                        {

                                                            foreach (var dk in allpieces)
                                                            {
                                                                int postxxk = dk.Location.X / dk.Width;
                                                                int postyyk = dk.Location.Y / dk.Width;
                                                                if (!flip)
                                                                {
                                                                    postxxk = 8 - postxxk;
                                                                    postyyk = 8 - postyyk;
                                                                }
                                                                else
                                                                {
                                                                    postxxk += 1;
                                                                    postyyk += 1;
                                                                }
                                                                foreach (var cck in dk.Controls)
                                                                {
                                                                    try
                                                                    {
                                                                        Button lk = (Button)cck;
                                                                        // bool notdff = false;
                                                                        //foreach (var sk in m)
                                                                        {
                                                                            if (s.position2[0] == postxxk && s.position2[1] == postyyk)
                                                                            {



                                                                                FieldInfo f1f = typeof(Control).GetField("EventClick",
                                                                                BindingFlags.Static | BindingFlags.NonPublic);

                                                                                object obf = f1f.GetValue(lk);
                                                                                PropertyInfo pf = lk.GetType().GetProperty("Events",
                                                                                    BindingFlags.NonPublic | BindingFlags.Instance);
                                                                                EventHandlerList list1f = (EventHandlerList)pf.GetValue(lk, null);
                                                                                list1f.RemoveHandler(obf, list1f[obf]);
                                                                                foreach (var pp in b.pieces)
                                                                                {
                                                                                    if (pp.x != postxxk || pp.y != postyyk)
                                                                                    {
                                                                                        continue;
                                                                                    }
                                                                                    // notdf = true;
                                                                                    lk.Text = pp.ToString();
                                                                                    lk.Size = new Size(dd.Width * 8 / 9, dd.Width * 6 / 9);
                                                                                    this.Text = ll.Location.X + " " + ll.Location.Y;
                                                                                    lk.BringToFront();
                                                                                    lk.Show();
                                                                                    lk.Name = "piece";
                                                                                    lk.BackColor = Color.Black;
                                                                                    lk.ForeColor = Color.Yellow;
                                                                                    if (pp.side == 0)
                                                                                    {
                                                                                        lk.BackColor = Color.Wheat;
                                                                                        lk.ForeColor = Color.Black;
                                                                                    }
                                                                                    lk.Click += delegate
                                                                                    {
                                                                                        viewposition(b);
                                                                                        getclicks(pp, postxxk, postyyk, lk, b);
                                                                                    };
                                                                                    //  ll.ForeColor = Color.White;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    catch { }
                                                                }
                                                            }
                                                        }

                                                        l.Name = "piece";
                                                        ll.Name = "";
                                                        ll.Text = "";
                                                        FieldInfo vff1 = typeof(Control).GetField("EventClick",
                                                        BindingFlags.Static | BindingFlags.NonPublic);

                                                        object vobjf = vff1.GetValue(l);
                                                        PropertyInfo vpif = l.GetType().GetProperty("Events",
                                                            BindingFlags.NonPublic | BindingFlags.Instance);
                                                        EventHandlerList vlistf = (EventHandlerList)vpif.GetValue(l, null);
                                                        listf.RemoveHandler(vobjf, vlistf[vobjf]);
                                                        l.Click += delegate {
                                                            viewposition(b);
                                                            getclicks(p, p.x, p.y, l, b);
                                                        };
                                                        viewposition(b);
                                                    };
                                                    //
                                                }
                                            }
                                            catch { }
                                        }
                                    }


                                }
                            };
                            //  ll.ForeColor = Color.White;
                        }
                        if (!notdf)
                        {
                            ll.Hide();
                        }
                        /* Label l = new Label();
                         l.Location = d.Location;
                         l.AutoSize = true;
                         l.Text= b.pieces[count].ToString();
                         l.BringToFront();
                         l.Show();
                         l.ForeColor = Color.Yellow;
                         iboard.Controls.Add(l); */
                    }
                    catch
                    {

                    }
                }

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            flip = !flip;
            button1.PerformClick();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
