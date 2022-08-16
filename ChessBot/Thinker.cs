using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows.Forms;

namespace ChessBot
{

    public enum pieceType { King, Queen, Rook, Bishop, knight, Pawn, nullpiece };
    public class Thinker
    {
        List<board> boards = new List<board>();
        List<board> boards2 = new List<board>();
        Dictionary<double, situation> dicts = new Dictionary<double, situation>();
        List<situation> dictsall = new List<situation>();
        bool stopit = false;
        //    List< situation> dictstwo = new List< situation>();
        // int alldone = 0;
        bool Irunhere = false;
        public void runCollectors()
        {
            Thread th = new Thread(() => collect());
            th.Start();
        }
        void collect()
        {
            while (true)
            {
                GC.Collect();
             //   GC.WaitForPendingFinalizers();
                Thread.Sleep(100);
            }

        }
        public void runallthreads()
        {
            Irunhere = true;
            for(int i = 0; i < 180; i++)
            {

                Thread th = new Thread(()=>  threadstock());
                th.Start();
            }
            MessageBox.Show("app started !");
        }
        public void stopthreads()
        {
            stopit = true;
        }
        public void calc(situation father, situation stmain, int maxdepth, counter cc, int side, bool dothreads, bool dothreads2)
        {
            if (maxdepth < 1 || (father != null && stmain.parent != father))
            {
                cc.i++;
                return;
            }
            var l = stmain.getallmoves();
            counter co = new counter();
            int i = 0;
            foreach (var a in l)
            {
                if (dothreads2)
                {
                    Thread th = new Thread(() => calcsinglmove(stmain, a, co));
                    th.Start();
                 /*   container2 ct = new container2();
                    ct.stm = stmain;
                    ct.mv = a;
                    ct.co = co;
                    addContainer2(ct);*/
                }
                else
                {
                    calcsinglmove(stmain, a, co);
                }
                i++;
            }
         //   if (!dothreads2)
            {
                while (co.i < i)
                {

                }
            }
            stmain.current = null;

            counter cc2 = new counter();
            i = 0;
            if (maxdepth > 1)
            {
                foreach (var j in stmain.childs)
                {
                    if (dothreads)
                    {
                        // Thread th = new Thread(() => calc(stmain, j, maxdepth - 1, cc2, side, dothreads, dothreads2));
                        // th.Start();
                        container ct = new container();
                        ct.father = stmain;
                        ct.stmain = j;
                        ct.maxdepth = maxdepth - 1;
                        ct.cc = cc2;
                        ct.side = side;
                        ct.dothreads = dothreads;
                        ct.dothreads2 = dothreads2;
                        addContainer(ct);
                    }
                    else
                    {
                        calc(stmain, j, maxdepth - 1, cc2, side, dothreads, dothreads2);
                    }
                    i++;
                }
            }
            if (maxdepth % 2 == 0)
            {
                
            }
            //    while (cc2.i < i)
            {
                switch (maxdepth)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }

            }
            cc.i++;
        }
        object mylock = new object();
        bool add_to_dict(situation st, double key)
        {
            lock (mylock)
            {
                //   if (dicts.ContainsKey(key))
                {
                    //     return false;
                }
                //  dicts.Add(key, st);
              //  dictsall.Add(st);
                return true;
            }
        }
        object lock2 = new object();
        void add_to_childs(situation stm, situation child)
        {
            lock (lock2)
            {
                stm.childs.Add(child);
            }
        }
        public void wait()
        {
            while (!stopit)
            {
               
                if (!Irunhere && iall == 0 && containers.Count==0 && containers2.Count==0)
                {
                     stopit = true;
                }
              
            }
        }
        List<container> containers = new List<container>();
        List<container2> containers2 = new List<container2>();
        object containerLock = new object();
        container getfreecontainer()
        {
            lock (containerLock)
            {
                if (containers.Count > 0)
                {
                  //  for (int i = 0; i < containers.Count; i++)
                    {
                        var f = containers[0]; //containers.Count - 1
                        containers.Remove(f);
                        return f;
                    }
                }
            }
            return null;
        }
        object containerLock2 = new object();
        container2 GetContainer2()
        {
            lock (containerLock2)
            {
                if (containers2.Count > 0)
                {
                    var f = containers2[containers2.Count - 1];
                    containers2.RemoveAt(containers2.Count - 1);
                    return f;
                }
            }
            return null;
        }
        object locker2 = new object();
        void addContainer(container cont)
        {
            lock (containerLock)
            {
                containers.Add(cont);
            }
        }
        object locker22 = new object();
        void addContainer2(container2 cont)
        {
            lock (locker22)
            {
                containers2.Add(cont);
            }
        }
        int iall = 0;
        object lock1 = new object();
        void setcounter(int a)
        {
            lock (lock1)
            {
                iall+=a;
            }
        }
        object stopobj = new object();
        bool getstop()
        {
            lock(stopobj)
            {
                return stopit;
            }
        }
         void threadstock()
        {
            bool stf = getstop();
              while (!stf)
            {

                var j2 = GetContainer2();
                if (j2 != null)
                {
                    setcounter(1);
                    calcsinglmove(j2.stm, j2.mv, j2.co);
                    setcounter(-1);
                    Irunhere = false;
                    continue;
                }
                var j = getfreecontainer();
                if (j != null)
                {
                    setcounter(1);
                    calc(j.father, j.stmain, j.maxdepth, j.cc, j.side, j.dothreads, j.dothreads2);
                    setcounter(-1);
                    Irunhere = false;
                }
                else if (!Irunhere && iall==0)
                {
                   // stopit = true;
                }
                stf = getstop();
                //Thread.Sleep(1);
            }
        }
        void calcsinglmove(situation stm, move mv, counter co)
        {
            var newboard = stm.current.copyme(mv);
            var m = newboard.copiedmove;
            //newboard.setsquares();
            newboard.aply_move(m);
            situation st = new situation();
            st.current = newboard;
            st.parent = stm;
            st.moveofme = m;
            st.sidePlayed = 1 - stm.sidePlayed;
      //      if (dicts.ContainsKey(newboard.key))
            {

            }
            newboard.calcKey();
          /*  if (!add_to_dict(st, newboard.key))
            {
                var dt = dicts[newboard.key];
                add_to_childs(stm, dt);
                co.i++;
                return;
            }*/
            newboard.putpiecesinsquares();
            newboard.getchecks();
            add_to_childs(stm, st);
            stm.numofmoves++;
            co.i++;
            /*      counter cob = new counter();
                  int i = 0;
                  foreach(var s in st.getallmoves())
                  {
                      i++;
                  }
                  while (cob.i < i)
                  {

                  }
                  cob.i++;*/
        }
    }
    public class container{
        public situation father;public situation stmain;public int maxdepth;public counter cc;public int side; public bool dothreads; public bool dothreads2;
    }
    public class container2
    {
        public situation stm; public move mv; public counter co;
    }
    public class counter
    {
        public int i = 0;
        
    }
    public class evaluation
    {
        public int eval;
        public bool calculated;
        public List<evaluation> evalswaiting=new List<evaluation>();
    }
    public class situation
    {
        public board current;
        public situation parent;
        public move moveofme;
        public List<situation> childs = new List<situation>();
     //   public move moveofOponnent;
        public evaluation tevaluation = new evaluation();
        public int sidePlayed=0;//0,1
        object locks = new object();
        int maxmoves=0;
        int nmv=0;
        public  int numofmoves {
            get
            {
                return nmv;
            }

            set {

                lock (locks)
                {
                    nmv = value;
                }
            } }
        public List<move> getallmoves()
        {
            List<move> moves = new List<move>();
            foreach(var p in current.pieces)
            {
                if (p.side != sidePlayed)
                {
                    moves.AddRange(p.getmove());
                }
            }
            maxmoves = moves.Count;
            return moves;
        }
    }
    public class board
    {
        public List<Piece> pieces;
        public square[,] squares;
        int n = 32;
        int rows = 8;
        int cols = 8;
        public double key;
        public bool white_to_move = true;
        double materialcount = 0;
        public bool white_o_o=true;
        public  bool white_o_o_o=true;
        public  bool black_o_o=true;
        public  bool black_o_o_o=true;
        public move copiedmove;
        public board(int numpieces=32,bool dot=true)
        {
            if (dot) 
            {
                pieces = new List<Piece>();
                setsquares();
                n = numpieces;
            }
        }
        public board copyme(move m)
        {
            board b = new board(dot : false);
            b.white_o_o = white_o_o;
            b.white_o_o_o = white_o_o_o;
            b.black_o_o = black_o_o;
            b.black_o_o_o = black_o_o_o;
            List<Piece> allpieces=new List<Piece>();
            move mv = new move();
           // int[] ii = { m.position1[0], m.position1[1] };
           // int[] ii2 = { m.position2[0], m.position2[1] };
            mv.position1 =  m.position1;
            mv.position2 = m.position2;// ii2;
            mv.promote = m.promote;
            mv.promotion = m.promotion;
            mv.itscastle = m.itscastle;
            mv.longmove = m.longmove;
            
            foreach(var p in pieces)
            {
                var pp = p.copyme(b);
                if (pp.x == m.part1.x && pp.y == m.part1.y)
                {
                    mv.part1 = pp;
                }
                else if(m.part2!=null&& pp.x == m.part2.x && pp.y == m.part2.y)
                {
                    mv.part2 = pp;
                }
                switch (pp.mytype)
                {
                    case pieceType.King:
                        switch (pp.side)
                        {
                            case 0:
                                b.whiteking = pp;
                                break;
                            case 1:
                                b.blackking = pp;
                                break;
                        }
                        break;
                }
                allpieces.Add(pp);
            }
            b.pieces = allpieces;
            b.setsquares();
            b.copiedmove = mv;
            return b;
        }
        public void setsquares()
        {
            squares = new square[rows+1, cols+1];
            for (int i = 0; i < cols; i++)
            {
                for (int ii = 0; ii < rows; ii++)
                {
                    squares[i+1, ii+1] = new square();
                }
            }
        }
        public List<int[]> getlegalmoves()
        {
            List<int[]> moves = new List<int[]>();

            return moves;
        }
        public void aply_move(move mv)
        {
            squares[mv.part1.x, mv.part1.y].pin = Piece.getnullpiece();
            if (mv.longmove)
            {
                mv.part1.longmovefirst = true;
            }
            foreach(var l in pieces)
            {
                if (l != mv.part1)
                {
                    l.longmovefirst = false;
                }
            }
            if (mv.promote)
            {
                mv.part1.mytype = mv.promotion;
            }
            squares[mv.part1.x, mv.part1.y].setme();
            mv.part1.x = mv.position1[0];
            mv.part1.y = mv.position1[1];
            mv.part1.nevermoved = false;

            if (mv.part2 != null)
            {
                mv.part2.nevermoved = false;
                if (mv.position2 == null)
                {
                    pieces.Remove(mv.part2);
                    squares[mv.part2.x, mv.part2.y].pin = Piece.getnullpiece(); 
                    squares[mv.part2.x, mv.part2.y].setme();
                }
                else
                {
                    squares[mv.part2.x, mv.part2.y].pin = Piece.getnullpiece(); 
                    squares[mv.part2.x, mv.part2.y].setme();
                    mv.part2.x = mv.position2[0];
                    mv.part2.y = mv.position2[1];
                    squares[mv.part2.x, mv.part2.y].pin = mv.part2;
                    squares[mv.part2.x, mv.part2.y].setme();
                }
            }
            squares[mv.part1.x, mv.part1.y].pin = mv.part1;
            squares[mv.part1.x, mv.part1.y].setme();
        }
        public double getkeyofposition(int x,int y,int side,double vv)
        {
            if (side == 0)
            {
                return Math.Sqrt(x * y + x +(y+9)*(y+9)+ y*Math.Sqrt(vv));
            }
            else
            {
                return Math.Sqrt(x * y*2 + (x+9)*(x+9) +y + Math.Sqrt(vv)*3);
            }
        }
        public void calcKey()
        {
            double ans = 0;
            materialcount = 0;
            foreach(var p in pieces)
            {
                double vv = p.getvalue();
                var jh = getkeyofposition(p.x,p.y,p.side,vv)*vv;
                ans += jh ;
                
               // ans += vv;
                materialcount += vv;
                if (p.side == 1)
                {
                 //   ans += vv*1378428;
                    materialcount -= vv * 2;
                }
                
            }
            if (white_to_move)
            {
                ans += 70;
            }
            if (white_o_o)
            {
                ans += 324;
            }
            if (white_o_o_o)
            {
                ans += 6548;
            }
            if (black_o_o)
            {
                ans += 944348;
            }
            if (black_o_o_o)
            {
                ans += 5657833;
            }
            key = ans;
        }
         Piece whiteking;
         Piece blackking;
        public Piece getwhiteking (){
            return whiteking;// pieces[3];
        }
        public Piece getblakking()
        {
            return blackking;// pieces[11];
        }
        public void putpiecesinsquares()
        {
            foreach(var p in pieces)
            {
                squares[p.x, p.y].pin = p;
                squares[p.x, p.y].setme();
            }
        }
        public void putpieces()
        {
            //white
            setsquares();
            int ie = 0;
            for (int i2 = 0; i2 < 2; i2++)
            {
                Piece p = null;
                int pblus = 1;
                int pos = 1;
                if (i2 == 1)
                {
                    pblus = -1;
                    pos = 8;
                }
                for (int i = 0; i < n / 2; i++)
                {

                    switch (i)
                    {
                        case 0:
                            p = new Piece(pieceType.Rook, pos, pos, i2);
                            break;
                        case 1:
                            p = new Piece(pieceType.knight, pos, pos+pblus*i, i2);
                            break;
                        case 2:
                            p = new Piece(pieceType.Bishop, pos, pos + pblus * i, i2);
                            break;

                        case 4:
                            p = new Piece(pieceType.Queen, pos, pos + pblus * i, i2);
                            break;
                        case 3:
                            p = new Piece(pieceType.King, pos, pos + pblus * i, i2);
                            switch (i2)
                            {
                                case 0:
                                    whiteking = p;
                                    break;

                            }
                            break;
                        case 5:
                            p = new Piece(pieceType.Bishop, pos, pos + pblus * i, i2);
                            break;
                        case 6:
                            p = new Piece(pieceType.knight, pos, pos + pblus * i, i2);
                            break;
                        case 7:
                            p = new Piece(pieceType.Rook, pos, pos + pblus * i, i2);
                            break;
                        default:
                            p = new Piece(pieceType.Pawn, pos+pblus, pos + pblus * (i-8), i2);
                            break;
                    }
                    if (i2 == 1 && i==3)
                    {
                        p = new Piece(pieceType.Queen, pos, pos + pblus * i, i2);
                    }
                    else if (i2 == 1 && i == 4)
                    {
                        p = new Piece(pieceType.King, pos, pos + pblus * i, i2);
                        blackking = p;
                        
                    }
                    int xx = p.x;
                    p.x = p.y;
                    p.y = xx;
                    if (i2==0) {
                        squares[p.x, p.y].checkedwhite = true;
                        squares[p.x, p.y].ocupiedwhite = true;
                        squares[p.x, p.y].pin = p;
                        if (p.mytype==pieceType.Rook || p.mytype == pieceType.Queen)
                        {
                            squares[p.x, p.y].rowcheck_w = true;
                        }
                        if (p.mytype == pieceType.Bishop || p.mytype == pieceType.Queen)
                        {
                            squares[p.x, p.y].colcheck_w = true;
                        }
                        if (p.mytype == pieceType.Pawn)
                        {
                            squares[p.x, p.y].occupiedwithwhitepawn = true;
                        }
                     }
                    else
                    {
                        squares[p.x, p.y].checkedblack = true;
                        squares[p.x, p.y].ocupiedblack = true;
                        squares[p.x, p.y].pin = p;
                        if (p.mytype == pieceType.Rook || p.mytype == pieceType.Queen)
                        {
                            squares[p.x, p.y].rowcheck_b = true;
                        }
                        if (p.mytype == pieceType.Bishop || p.mytype == pieceType.Queen)
                        {
                            squares[p.x, p.y].colcheck_b = true;
                        }
                        if (p.mytype == pieceType.Pawn)
                        {
                            squares[p.x, p.y].occupiedwithblackpawn = true;
                        }
                    }
                    if (i2 == 0) {
                        squares[p.x, p.y].ocupiedwhite = true;
                    }
                    else
                    {
                        squares[p.x, p.y].checkedblack = true;
                    }
                    p.nb = this;
                    pieces.Add(p);
                    ie++;
                }
                
            }
        }
        public void getchecks()
        {
            foreach(var p in pieces)
            {
                p.getchecks();
            }

        }
        public void resetsquares()
        {
            foreach (var s in squares)
            {
                if (s != null)
                {
                    s.reset();
                }
            }
        }
        void getchecks_white(Piece p)
        {
            
        }
        public double eval(int side)
        {
            double ans = 0;
            foreach(var l in pieces)
            {
                if (l.exists)
                {
                    if (l.side == side)
                    {
                        ans += l.getvalue();
                    }
                    else
                    {
                        ans -= l.getvalue();
                    }
                }
            }
            return ans;
        }
    }
    public class Piece
    {
        public pieceType mytype;
        public int x;
        public  int y;
        public bool exists = false;
        public int side = 0;
        public board nb;
        public bool longmovefirst = false;
        public bool nevermoved = true;
        public int xi { get { return x - 1; } }
        public int yi { get { return x - 1; } }
        public checkrecord chck;
        public Piece copyme(board bb)
        {
            Piece pc = new Piece(mytype,x,y,side);
          /*  pc.mytype = mytype;
            pc.x = x;
            pc.y = y;
            pc.side = side;*/
            pc.nb = bb;
            pc.longmovefirst = longmovefirst;
            pc.nevermoved = nevermoved;
            return pc;

        }
        bool acceptance(move mv,Piece king)
        {
            if (king.chck == null || mv.part1.mytype==pieceType.King || mv.part2==king.chck.pchceck)
            {
                if (king.chck != null&&king.chck.numpieces>1 && mv.part1.mytype != pieceType.King)
                {
                    return false;
                }
                return true;
            }
            if (king.chck.numpieces > 1)
            {
                return false;
            }
            switch (king.chck.typecheck)
            {
                case 0:
                    if (mv.position1 == king.chck.positionofcheck)
                    {
                        return true;
                    }
                    break;
                case 1:
                    return king.chck.match_rook(mv.position1);
                case 2:
                    return king.chck.match_bishop(mv.position1);
            }
            return false;
        }
        public Piece (pieceType type, int x, int y,int side)
        {
            mytype = type;
            this.x = x;
            this.y = y;
            this.side = side;
        }
        public static Piece getnullpiece()
        {
            Piece p = new Piece(pieceType.nullpiece, -1, -1, -1);
            return p;
        }
        public override string ToString()
        {
            return mytype.ToString();
        }
        public int getvalue()
        {
            int ans = 0;
            switch (mytype)
            {
                case pieceType.King:
                    ans = 4;
                    break;
                case pieceType.Queen:
                    ans = 90;
                    break;
                case pieceType.Rook:
                    ans = 50;
                    break;
                case pieceType.Bishop:
                    ans = 35;
                    break;
                case pieceType.knight:
                    ans = 30;
                    break;
                case pieceType.Pawn:
                    ans = 10;
                    break;
            }
            return ans;
        }
        public void getchecks()
        {
            switch (mytype)
            {
                case pieceType.King:
                    getkingchecks();
                    break;
                case pieceType.Queen:
                    getrookchecks();
                    getbishopchecks();
                    break;
                case pieceType.Rook:
                    getrookchecks();
                    break;
                case pieceType.Bishop:
                    getbishopchecks();
                    break;
                case pieceType.knight:
                    getknightchecks();
                    break;
                case pieceType.Pawn:
                    getpawnchecks();
                    break;
            }
            
        }
        void getbishopchecks()
        {
            
            int aa = 1;
            int bb = 1;
            //
            int a1 = 0;
            int a2 = 9;
            int b1 = 0;
            int b2 = 9;
            int ttx = x + 1;
            int tty = y + 1;
            int typecheck = 1;
            for (int ix = 0; ix < 4; ix++)
            {
                switch (ix)
                {
                    case 1:
                        aa = -1;
                        bb = 1;
                        //
                        ttx = x - 1;
                        tty = y + 1;
                        //
                        a1 = 0;
                        a2 = 9;
                        b1 = 0;
                        b2 = 9;
                        typecheck = 0;
                        break;
                    case 2:
                        aa = -1;
                        bb = -1;
                        //
                        ttx = x - 1;
                        tty = y - 1;
                        //
                        a1 = 0;
                        a2 = 9;
                        b1 = 0;
                        b2 = 9;
                        typecheck = 0;
                        break;
                    case 3:
                        aa = 1;
                        bb = -1;
                        //
                        ttx = x + 1;
                        tty = y - 1;
                        //
                        a1 = 0;
                        a2 = 9;
                        b1 = 0;
                        b2 = 9;
                        typecheck = 1;
                        break;
                }
                if (y == 4)
                {

                }
                for (int i = ttx, i2 = tty; i > a1 && i < a2 && i2 < b2 && i2 > b1; i += aa, i2 += bb)
                {
                    switch (side)
                    {
                        case 0:
                            nb.squares[i, i2].checkedwhite = true;
                            switch (typecheck)
                            {
                                case 0:
                                    nb.squares[i, i2].diagonalcheck_w_r = true;
                                    break;
                                case 1:
                                    nb.squares[i, i2].diagonalcheck_w_l = true;
                                    break;
                            }
                            break;
                        case 1:
                            nb.squares[i, i2].checkedblack = true;
                            switch (typecheck)
                            {
                                case 0:
                                    nb.squares[i, i2].diagonalcheck_b_r = true;
                                    break;
                                case 1:
                                    nb.squares[i, i2].diagonalcheck_b_l = true;
                                    break;
                            }
                            break;
                    }
                    if (nb.squares[i, i2].isocupied())
                    {
                        nb.squares[i, i2].activatecheck(this);
                        break;
                    }

                }
            }
           
        }
        void getknightchecks()
        {
            for (int i = 1; i < 9; i++)
            {
                int adx = -1;
                int ady = -1;
                switch (i)
                {
                    case 1:
                        adx = -1;
                        ady = 2;
                        break;
                    case 2:
                        adx = -1;
                        ady = -2;
                        break;
                    case 3:
                        adx = 1;
                        ady = -2;
                        break;
                    case 4:
                        adx = 1;
                        ady = 2;
                        break;
                    case 5:
                        ady = 1;
                        adx = 2;
                        break;
                    case 6:
                        ady = 1;
                        adx = -2;
                        break;
                    case 7:
                        ady = -1;
                        adx = 2;
                        break;
                    case 8:
                        ady = -1;
                        adx = -2;
                        break;
                }

                int[] n = { x + adx, y + ady };
                if (n[0] > 8 || n[0] < 1 || n[1] > 8 || n[1] < 1)
                {
                    continue;
                }
                nb.squares[n[0], n[1]].activatecheck(this);
                switch (side)
                {
                    case 0:
                        nb.squares[n[0], n[1]].checkedwhite = true;
                        break;
                    case 1:
                        nb.squares[n[0], n[1]].checkedblack = true;
                        break;
                }
            }
        }
        void getrookchecks()
        {
            int aa = 1;
            int bb = 0;
            //
            int a1 = 0;
            int a2 = 9;
            int b1 = 0;
            int b2 = 10;
            int ttx = x + 1;
            int tty = y;
            int typecheck = 0;
            for (int ix = 0; ix < 4; ix++)
            {
                switch (ix)
                {
                    case 1:
                        aa = -1;
                        bb = 0;
                        //
                        ttx = x - 1;
                        //
                        a1 = 0;
                        a2 = 9;
                        b1 = 0;
                        b2 = 9;
                        break;
                    case 2:
                        aa = 0;
                        bb = 1;
                        //
                        ttx = x;
                        tty = y + 1;
                        //
                        a1 = 0;
                        a2 = 10;
                        b1 = 0;
                        b2 = 9;
                        typecheck = 1;
                        break;
                    case 3:
                        aa = 0;
                        bb = -1;
                        //
                        tty = y - 1;
                        //
                        a1 = 0;
                        a2 = 10;
                        b1 = 0;
                        b2 = 9;
                        break;
                }
                for (int i = ttx, i2 = tty; i > a1 && i < a2 && i2 < b2 && i2 > b1; i += aa, i2 += bb)
                {
                    switch (side) {
                        case 0:
                            nb.squares[i, i2].checkedwhite = true;
                        switch (typecheck)
                        {
                                case 0://row
                                    nb.squares[i, i2].rowcheck_w = true;
                                    break;
                                case 1:
                                    nb.squares[i, i2].colcheck_w = true;
                                    break;
                        }
                            break;
                        case 1:
                            nb.squares[i, i2].checkedblack = true;
                            switch (typecheck)
                            {
                                case 0://row
                                    nb.squares[i, i2].rowcheck_b = true;
                                    break;
                                case 1:
                                    nb.squares[i, i2].colcheck_b = true;
                                    break;
                            }
                            break;
                    }
                    if (nb.squares[i, i2].isocupied())
                    {
                        nb.squares[i, i2].activatecheck(this);
                        break;
                    }
                }
            }
        }
        void getkingchecks()
        {
            for (int i = -1; i < 2; i++)
            {
                for (int i2 = -1; i2 < 2; i2++)
                {
                    var xv = x + i;
                    var yv = y + i2;
                    if (xv < 1 || yv < 1 || yv > 8 || xv > 8)
                    {
                        continue;
                    }
                    switch (side)
                    {
                        case 0:
                            nb.squares[xv, yv].checkedwhite = true;
                            break;
                        case 1:
                            nb.squares[xv, yv].checkedblack = true;
                            break;
                    }
                }
            }
         }
         void getpawnchecks()
        {
            switch (side)
            {
                case 0:
                    if ( y < 8)
                    {
                        if (x < 8)
                        { nb.squares[x + 1, y + 1].checkedwhite = true;
                            nb.squares[x + 1, y + 1].activatecheck(this);
                        }
                        if (x > 1)
                        {
                            nb.squares[x - 1, y + 1].checkedwhite = true;
                            nb.squares[x - 1, y + 1].activatecheck(this);
                        }
                    }
                    break;
                case 1:
                    if (y > 1)
                    {
                        if (x < 8)
                        { nb.squares[x + 1, y - 1].checkedblack = true;
                            nb.squares[x + 1, y - 1].activatecheck(this);
                        }
                        if (x > 1)
                        {
                            nb.squares[x - 1, y - 1].checkedblack = true;
                            nb.squares[x - 1, y - 1].activatecheck(this);
                        }
                    }
                    break;
            }
        }
        public List<move> getknightmoves()
        {
            List<move> moves = new List<move>();
            var rawalow = !rowcheck();
            var collow = !colcheck();
            var diaglow_R = !diagonal_right_check();
            var diaglow_L = !diagonal_left_check();
            for (int i = 1; i < 9; i++)
            {
                int adx = -1;
                int ady = -1;
                switch(i){
                    case 1:
                        adx = -1;
                        ady = 2;
                        break;
                    case 2:
                        adx = -1;
                        ady = -2;
                        break;
                    case 3:
                        adx = 1;
                        ady = -2;
                        break;
                    case 4:
                        adx = 1;
                        ady = 2;
                        break;
                    case 5:
                        ady = 1;
                        adx = 2;
                        break;
                    case 6:
                        ady = 1;
                        adx = -2;
                        break;
                    case 7:
                        ady = -1;
                        adx = 2;
                        break;
                    case 8:
                        ady = -1;
                        adx = -2;
                        break;
                }
                    
                    int[] n = {x+adx,y+ady };
                    if (n[0]>8 || n[0]<1 || n[1]>8 || n[1] < 1)
                    {
                     continue;
                    }
                    move mv = new move();
                    mv.part1 = this;
                    mv.position1 = n;
                    switch (side)
                    {
                        case 0:
                            if (!nb.squares[n[0], n[1]].ocupiedwhite&& rawalow&& collow&& diaglow_L&& diaglow_R)
                            {
                            if (nb.squares[n[0], n[1]].ocupiedblack)
                            {
                                
                                mv.part2 = nb.squares[n[0], n[1]].pin;
                            }
                                moves.Add(mv);
                            }
                            break;
                        case 1:
                            if (!nb.squares[n[0], n[1]].ocupiedblack && rawalow && collow && diaglow_L && diaglow_R)
                            {
                            if (nb.squares[n[0], n[1]].ocupiedwhite)
                            {
                               // nb.squares[n[0], n[1]].activatecheck(this);
                                mv.part2 = nb.squares[n[0], n[1]].pin;
                            }
                            moves.Add(mv);
                        }
                        break;
                    }
                
            }
            return moves;
        }
        public List<move> getrookmove()
        {
            List<move> moves = new List<move>();
            var rawalow = !rowcheck();
            var collow = !colcheck();
            var diaglow_R = !diagonal_right_check();
            var diaglow_L = !diagonal_left_check();
            bool allow = collow&&diaglow_L&&diaglow_R;
            int aa = 1;
            int bb = 0;
            //
            int a1 = 0;
            int a2 = 9;
            int b1 = 0;
            int b2 = 10;
            int ttx = x+1;
            int tty = y;
            for (int ix = 0; ix < 4; ix++)
            {
                switch (ix)
                {
                    case 1:
                        aa = -1;
                        bb = 0;
                        //
                        ttx = x - 1;
                        //
                        a1 = 0;
                        a2 = 9;
                        b1 = 0;
                        b2 = 9;
                        break;
                    case 2:
                        aa = 0;
                        bb = 1;
                        //
                        ttx = x;
                        tty = y + 1;
                        //
                        a1 = 0;
                        a2 = 10;
                        b1 = 0;
                        b2 = 9;
                        allow = rawalow && diaglow_L && diaglow_R;
                        break;
                    case 3:
                        aa = 0;
                        bb = -1;
                        //
                        tty = y - 1;
                        //
                        a1 = 0;
                        a2 = 10;
                        b1 = 0;
                        b2 = 9;
                        break;
                }
                for (int i = ttx, i2 = tty; i > a1 && i < a2 && i2 < b2 && i2 > b1; i += aa, i2 += bb)
                {
                    if (!allow)
                    {
                        break;
                    }
                    move mv = new move();
                    mv.part1 = this;
                    int[] mk = { i, i2 };
                    mv.position1 = mk;
                    if (nb.squares[i, i2].isocupied())
                    {
                        mv.part2 = nb.squares[i, i2].pin;
                        if (side == 0 && nb.squares[i, i2].ocupiedblack)
                        {
                           
                            moves.Add(mv);
                        }
                        else if (side == 1 && nb.squares[i, i2].ocupiedwhite)
                        {
                            //nb.squares[i, i2].activatecheck(this);
                            moves.Add(mv);
                        }
                        break;
                    }
                    moves.Add(mv);
                }
            }

            return moves;
        }
        public List<move> getbishopmove()
        {
            List<move> moves = new List<move>();
            var rawalow = !rowcheck();
            var collow = !colcheck();
            var diaglow_R = !diagonal_right_check();
            var diaglow_L = !diagonal_left_check();
            bool allow = diaglow_R && rawalow && collow;
            int aa = 1;
            int bb = 1;
            //
            int a1 = 0;
            int a2 = 9;
            int b1 = 0;
            int b2 = 9;
            int ttx = x + 1;
            int tty = y + 1;
            for (int ix = 0; ix < 4; ix++)
            {
                switch (ix)
                {
                    case 1:
                        aa = -1;
                        bb = -1;
                        //
                        ttx = x - 1;
                        tty = y - 1;
                        //
                        a1 = 0;
                        a2 = 9;
                        b1 = 0;
                        b2 = 9;
                       
                        break;
                    case 2:
                        aa = -1;
                        bb = 1;
                        //
                        ttx = x - 1;
                        tty = y + 1;
                        //
                        a1 = 0;
                        a2 = 9;
                        b1 = 0;
                        b2 = 9;
                        allow = diaglow_L && rawalow && collow;
                        break;
                    case 3:
                        aa = 1;
                        bb = -1;
                        //
                        ttx = x + 1;
                        tty = y - 1;
                        //
                        a1 = 0;
                        a2 = 9;
                        b1 = 0;
                        b2 = 9;
                        break;
                }
                for (int i = ttx, i2 = tty; i > a1 && i < a2 && i2 < b2 && i2 > b1; i += aa, i2 += bb)
                {
                    if (!allow)
                    {
                        break;
                    }
                    move mv = new move();
                    mv.part1 = this;
                    int[] mk = { i, i2 };
                    mv.position1 = mk;
                    if (nb.squares[i, i2].isocupied())
                    {
                        mv.part2 = nb.squares[i, i2].pin;
                        if (side == 0 && nb.squares[i, i2].ocupiedblack)
                        {
                            
                            moves.Add(mv);
                        }
                        else if (side == 1 && nb.squares[i, i2].ocupiedwhite)
                        {
                            
                            moves.Add(mv);
                        }
                        break;
                    }
                    moves.Add(mv);
                }
            }
            return moves;
        }
        public List<move> getkingmoves()
        {
            List<move> moves = new List<move>();
            //var k = nb.getblakking();
            for(int i = -1; i < 2; i++)
            {
                for(int i2 = -1; i2 < 2; i2++)
                {
                    var xv = x + i;
                    var yv = y + i2;
                    if (xv<1 || yv<1 || yv>8 || xv > 8)
                    {
                        continue;
                    }
                    move mv = new move();
                    mv.part1 = this;
                    int[] n = { xv, yv };
                    mv.position1 = n;
                    bool fok = true;
                    if (chck != null)
                    {
                       // if (!))
                        {
                            fok = mv.match_bishop() && mv.match_rook();
                        }
                    }
                    if ((side==0 &&fok&& (!nb.squares[xv, yv].ocupiedwhite && !nb.squares[xv, yv].checkedblack)) || (side==1 &&fok&& !nb.squares[xv, yv].ocupiedblack && !nb.squares[xv, yv].checkedwhite))
                    {
                        if(nb.squares[xv, yv].isocupied())
                        {
                            mv.part2 = nb.squares[xv, yv].pin;
                        }
                        moves.Add(mv);
                    }
                }

            }
            if (side == 0)
            {
                if (nb.white_o_o)
                {
                    bool cs = true;
                    for(int i = 1; i < 3; i++)
                    {
                        int tx = x - i;
                        if (tx<2 || nb.squares[tx, y].checkedblack || nb.squares[tx,y].isocupied()|| !nevermoved)
                        {
                            cs = false;
                            break;
                        }
                    }
                    int tx2 = x - 3;
                    if (cs && nb.squares[tx2, y].pin.mytype==pieceType.Rook && nb.squares[tx2, y].pin.nevermoved)
                    {
                        move m = new move();
                        m.part1 = this;
                        m.part2 = nb.squares[tx2, y].pin;
                        int[] n = { x-2, y };
                        m.position1 = n;
                        int[] n2 = { tx2 +2, y };
                        m.position2 = n2;
                        m.itscastle = 0;
                        moves.Add(m);
                    }
                }
                if (nb.white_o_o_o)
                {
                    bool cs = true;
                    for (int i = 1; i < 4; i++)
                    {
                        int tx = x + i;
                   //     if (nb.squares[tx, y] == null)
                        {
                     //       continue;
                        }
                        if (tx>7 || nb.squares[tx, y].checkedblack || nb.squares[tx, y].isocupied()||!nevermoved)
                        {
                            cs = false;
                            break;
                        }
                    }
                    int tx2 = x + 4;
                    if (cs && nb.squares[tx2, y].pin.mytype == pieceType.Rook && nb.squares[tx2, y].pin.nevermoved)
                    {
                        move m = new move();
                        m.part1 = this;
                        m.part2 = nb.squares[tx2, y].pin;
                        int[] n = { x + 2, y };
                        m.position1 = n;
                        int[] n2 = { tx2-3, y };
                        m.position2 = n2;
                        m.itscastle = 1;
                        moves.Add(m);
                    }
                }
            }
            else
            {
                if (nb.black_o_o)
                {
                    bool cs = true;
                    for (int i = 1; i < 3; i++)
                    {
                        int tx = x - i;
                        if (tx<2 || nb.squares[tx, y].checkedwhite || nb.squares[tx,y].isocupied() || !nevermoved)
                        {
                            cs = false;
                            break;
                        }
                    }
                    int tx2 = x - 3;
                    if (cs && nb.squares[tx2, y].pin.mytype == pieceType.Rook && nb.squares[tx2, y].pin.nevermoved)
                    {
                        move m = new move();
                        m.part1 = this;
                        m.part2 = nb.squares[tx2, y].pin;
                        int[] n = { x - 2, y };
                        m.position1 = n;
                        int[] n2 = { tx2 + 2, y };
                        m.position2 = n2;
                        m.itscastle = 2;
                        moves.Add(m);
                    }
                }
                if (nb.black_o_o_o)
                {
                    bool cs = true;
                    for (int i = 1; i < 4; i++)
                    {
                        int tx = x + i;
                       // if (nb.squares[x, tx] == null)
                        {
                         //   continue;
                        }
                        if (tx>7 || nb.squares[tx, y].checkedwhite || nb.squares[tx, y].isocupied() || !nevermoved)
                        {
                            cs = false;
                            break;
                        }
                    }
                    int tx2 = x + 4;
                    if (cs && nb.squares[tx2, y].pin.mytype == pieceType.Rook && nb.squares[tx2, y].pin.nevermoved)
                    {
                        move m = new move();
                        m.part1 = this;
                        m.part2 = nb.squares[tx2, y].pin;
                        int[] n = { x + 2, y };
                        m.position1 = n;
                        int[] n2 = { tx2 - 3, y };
                        m.position2 = n2;
                        m.itscastle = 3;
                        moves.Add(m);
                    }
                }
            }
            return moves;
        }
        
        public List<move> getpawnmove()
        {
            List<move> moves = new List<move>();
            var rawalow = !rowcheck();
            var collow = !colcheck();
            var diaglow_R = !diagonal_right_check();
            var diaglow_L = !diagonal_left_check();
            bool con = true;
            if (nevermoved)
            {
                move m = new move();
                m.part1 = this;
                if (side == 0) {
                    int[] mt = {x,y+2 };
                    m.position1 = mt;
                    if(nb.squares[mt[0],mt[1]].isocupied()|| nb.squares[mt[0], mt[1] - 1].isocupied())
                    {
                        con = false;
                    }
                }
                else
                {
                    int[] mt = { x, y - 2 };
                    m.position1 = mt;
                    if (nb.squares[mt[0], mt[1]].isocupied() || nb.squares[mt[0], mt[1] + 1].isocupied())
                    {
                        con = false;
                    }
                }
                //I need to check if it's safe // change in row number requires rowcheck ##
                if (con&&rawalow && diaglow_L&& diaglow_R && !nb.squares[m.position1[0], m.position1[1]].isocupied())
                {
                    m.longmove = true;
                    moves.Add(m);
                }
            }
            move mm = new move();
            mm.part1 = this;
            if (side == 0)
            {
                int[] mt = { x, y + 1 };
                mm.position1 = mt;
            }
            else
            {
                int[] mt = { x, y - 1 };
                mm.position1 = mt;
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_L && diaglow_R && !nb.squares[mm.position1[0], mm.position1[1]].isocupied())
            {
                moves.Add(mm);
            }
            move mf = new move();
            mf.part1 = this;
            bool addme = false;
            
            if (side == 0)
            {
                int[] mt = { x+1, y + 1 };
                mf.position1 = mt;

                if ((x < 8 && y < 8 ))
                {
                    //nb.squares[x + 1, y + 1].checkedwhite = true;
                    if (nb.squares[x + 1, y + 1].ocupiedblack)
                    {
                        addme = true;
                        
                        mf.part2 = nb.squares[x + 1, y + 1].pin;
                    }
                }

            }
            else
            {
                int[] mt = { x-1, y - 1 };
                mf.position1 = mt;
                if ((x >1 && y >1 ))
                {
                    //nb.squares[x - 1, y - 1].checkedblack = true;
                    if (nb.squares[x - 1, y - 1].ocupiedwhite)
                    {
                        addme = true;
                        
                        mf.part2 = nb.squares[x - 1, y - 1].pin;
                    }
                }
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_R &&collow && addme)
            {
                moves.Add(mf);
            }
            move mff = new move();
            mff.part1 = this;
            addme = false;
            if (side == 0)
            {
                int[] mt = { x - 1, y + 1 };
                mff.position1 = mt;
                if ((x >1 && y < 8 && nb.squares[x - 1, y + 1].ocupiedblack))
                {
                    addme = true;
                    
                    mff.part2 = nb.squares[x - 1, y + 1].pin;
                }
            }
            else
            {
                int[] mt = { x + 1, y - 1 };
                mff.position1 = mt;
                if ((x <8 && y > 1 && nb.squares[x + 1, y - 1].ocupiedwhite))
                {
                    addme = true;
                    
                    mff.part2 = nb.squares[x + 1, y - 1].pin;
                }
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_L && collow && addme)
            {
                moves.Add(mff);
            }
            //on pasent
            move mffk = new move();
            mffk.part1 = this;
            addme = false;
            if (side == 0)
            {
                int[] mt = { x - 1, y + 1 };
                mffk.position1 = mt;
                if ((x >1 && y < 8 && (nb.squares[x - 1, y].pin.mytype==pieceType.Pawn&& nb.squares[x - 1, y].pin.longmovefirst && nb.squares[x - 1, y].ocupiedblack)))
                {
                    addme = true;
                    mffk.part2 = nb.squares[x - 1, y].pin;
                }
            }
            else
            {
                int[] mt = { x + 1, y - 1 };
                mffk.position1 = mt;
                if ((x <8 && y > 1 && (nb.squares[x + 1, y].pin.mytype == pieceType.Pawn&& nb.squares[x + 1, y].pin.longmovefirst && nb.squares[x + 1, y].ocupiedwhite) ))
                {
                    addme = true;
                    mffk.part2 = nb.squares[x + 1, y].pin;
                }
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_L && collow && addme)
            {
                moves.Add(mffk);
            }
            //on pasent
            move mffl = new move();
            mffl.part1 = this;
            addme = false;
            if (side == 0)
            {
                int[] mt = { x + 1, y + 1 };
                mffl.position1 = mt;
                if ((x <8 && y < 8 && (nb.squares[x + 1, y].pin.mytype == pieceType.Pawn && nb.squares[x + 1, y].pin.longmovefirst&& nb.squares[x + 1, y].ocupiedblack)))
                {
                    addme = true;
                    mffl.part2 = nb.squares[x + 1, y].pin;
                }
            }
            else
            {
                int[] mt = { x - 1, y - 1 };
                mffl.position1 = mt;
                if ((x > 1 && y > 1 && (nb.squares[x - 1, y].pin.mytype == pieceType.Pawn&&  nb.squares[x - 1, y].pin.longmovefirst&& nb.squares[x - 1, y].ocupiedwhite)))
                {
                    addme = true;
                    mffl.part2 = nb.squares[x - 1, y].pin;
                }
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_L && collow && addme)
            {
                moves.Add(mffl);
            }
            //promote ###
            List<move> moves2 = new List<move>();
            int ypromote =0;
            switch (side)
            {
                case 0:
                    ypromote = 8;
                    break;
                case 1:
                    ypromote = 1;
                    break;
            }
            foreach (var mj in moves)
            {
                if (mj.position1[1] == ypromote)
                {
                    
                    for(byte i = 1; i < 5; i++)
                    {
                        move mvk = new move();
                        mvk.part1 = this;
                        mvk.position1 = mj.position1;
                        mvk.position2 = mj.position2;
                        mvk.part2 = mj.part2;
                        mvk.promote = true;
                        switch (i)
                        {
                            case 1:
                                mvk.promotion = pieceType.Bishop;
                                break;case 2:
                                mvk.promotion = pieceType.Rook;
                                break;case 3:
                                mvk.promotion = pieceType.knight;
                                break;case 4:
                                mvk.promotion = pieceType.Queen;
                                break;
                        }
                        moves2.Add(mvk);
                    }
                }
                else
                {
                    moves2.Add(mj);
                }
            }
            return moves2;

        }
        public bool colcheck()
        {
            if (side == 0)
            {
                var king = nb.getwhiteking();
                if (king.x != x)
                {
                    return false;//king will never be checked ##
                }
                bool righthandcheck = false;
                bool lefthandcheck = false;
                if ((y < 8 && nb.squares[x , y+1].colcheck_b&&!nb.squares[x, y + 1].isocupied()) || (nb.squares[x, y].colcheck_b && y < 8 && nb.squares[x , y + 1].rookc() && nb.squares[x , y + 1].pin.side != side && king.y<y))
                {
                    righthandcheck = true;
                }
                if ((y > 1 && nb.squares[x , y-1].colcheck_b && !nb.squares[x, y - 1].isocupied()) || (nb.squares[x, y].colcheck_b && y > 1 && nb.squares[x, y - 1].rookc() && nb.squares[x, y - 1].pin.side != side && king.y > y))
                {
                    lefthandcheck = true;
                }
                if (king.y > y && righthandcheck)
                {
                    return false;//it means that there is something blocking the check
                }
                else if (king.y < y && lefthandcheck)
                {
                    return false;
                }
                if (!(righthandcheck || lefthandcheck))
                {
                    return false;//there are no checks close to the piece ##
                }
                if (righthandcheck)
                {
                    bool undercheck = true;
                    for (int iy = y - 1; iy > king.y; iy--)
                    {
                        if (!nb.squares[x, iy].colcheck_b)
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
                if (lefthandcheck)
                {
                    bool undercheck = true;
                    for (int iy = y + 1; iy < king.y; iy++)
                    {
                        if (!nb.squares[x, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
            }
            else
            {
                var king = nb.getblakking();
                if (king.x != x)
                {
                    return false;//king will never be checked ##
                }
                bool righthandcheck = false;
                bool lefthandcheck = false;
                if ((y < 8 && nb.squares[x, y + 1].colcheck_w && !nb.squares[x, y + 1].isocupied()) || (nb.squares[x, y].colcheck_w && y < 8 && nb.squares[x, y + 1].rookc() && nb.squares[x, y + 1].pin.side != side && king.y < y))
                {
                    righthandcheck = true;
                }
                if ((y > 1 && nb.squares[x, y - 1].colcheck_w && !nb.squares[x, y - 1].isocupied()) || (nb.squares[x, y].colcheck_w && y > 1 && nb.squares[x, y - 1].rookc() && nb.squares[x, y - 1].pin.side != side && king.y > y))
                {
                    lefthandcheck = true;
                }
                if (king.y > y && righthandcheck)
                {
                    return false;//it means that there is something blocking the check
                }
                else if (king.y < y && lefthandcheck)
                {
                    return false;
                }
                if (!(righthandcheck || lefthandcheck))
                {
                    return false;//there are no checks close to the piece ##
                }
                if (righthandcheck)
                {
                    bool undercheck = true;
                    for (int iy = y - 1; iy > king.y; iy--)
                    {
                        if (!nb.squares[x, iy].colcheck_w)
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
                if (lefthandcheck)
                {
                    bool undercheck = true;
                    for (int iy = y + 1; iy < king.y; iy++)
                    {
                        if (!nb.squares[x, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        public bool rowcheck()
        {
            if (side == 0) {
                var king = nb.getwhiteking();
                if (king.y != y)
                {
                    return false;//king will never be checked ##
                }
                bool righthandcheck = false;
                bool lefthandcheck = false;
                if ((x < 8 && nb.squares[x + 1, y].rowcheck_b&&!nb.squares[x + 1, y].isocupied()) ||(nb.squares[x, y].rowcheck_b && x < 8&& nb.squares[x+1, y].rookc() && nb.squares[x+1, y].pin.side != side && king.x < x))
                {
                    righthandcheck = true;
                }
                if ((x > 1 && nb.squares[x - 1, y].rowcheck_b && !nb.squares[x - 1, y].isocupied()) || (nb.squares[x, y].rowcheck_b && x > 1&&nb.squares[x - 1, y].rookc() && nb.squares[x - 1, y].pin.side != side && king.x > x))
                {
                    lefthandcheck = true;
                }
                if (king.x > x && righthandcheck)
                {
                    return false;//it means that there is something blocking the check
                }
                else if (king.x < x && lefthandcheck)
                {
                    return false;
                }
                if(!(righthandcheck || lefthandcheck))
                {
                    return false;//there are no checks close to the piece ##
                }
                if (righthandcheck)
                {
                    bool undercheck = true;
                    for(int ix = x - 1; ix > king.x; ix--)
                    {
                        if (nb.squares[ix, y].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
                if (lefthandcheck)
                {
                    bool undercheck = true;
                    for (int ix = x + 1; ix < king.x; ix++)
                    {
                        if (nb.squares[ix, y].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
            }
            else
            {
                var king = nb.getblakking();
                if (king.y != y)
                {
                    return false;//king will never be checked ##
                }
                bool righthandcheck = false;
                bool lefthandcheck = false;
                if ((x < 8 && nb.squares[x + 1, y].rowcheck_w && !nb.squares[x + 1, y].isocupied()) ||(nb.squares[x, y].rowcheck_w && x < 8 && nb.squares[x + 1, y].rookc() && nb.squares[x + 1, y].pin.side != side && king.x < x))
                {
                    righthandcheck = true;
                }
                if ((x > 1 && nb.squares[x - 1, y].rowcheck_w && !nb.squares[x - 1, y].isocupied()) || (nb.squares[x, y].rowcheck_w && x > 1 && nb.squares[x - 1, y].rookc() && nb.squares[x - 1, y].pin.side != side && king.x> x))
                {
                    lefthandcheck = true;
                }
                if (king.x > x && righthandcheck)
                {
                    return false;//it means that there is something blocking the check
                }
                else if (king.x < x && lefthandcheck)
                {
                    return false;
                }
                if (!(righthandcheck || lefthandcheck))
                {
                    return false;//there are no checks close to the piece ##
                }
                if (righthandcheck)
                {
                    bool undercheck = true;
                    for (int ix = x - 1; ix > king.x; ix--)
                    {
                        if (nb.squares[ix, y].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
                if (lefthandcheck)
                {
                    bool undercheck = true;
                    for (int ix = x + 1; ix < king.x; ix++)
                    {
                        if (nb.squares[ix, y].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        public bool diagonal_left_check()
        {
            if (side == 0)
            {
                var king = nb.getwhiteking();
                if (king.x-x != king.y-y )
                {
                    return false;//king will never be checked ##
                }
                bool righthandcheck = false;
                bool lefthandcheck = false;
                if ((x < 8 &&y<8 && nb.squares[x + 1, y+1].diagonalcheck_b_r && !nb.squares[x + 1, y + 1].isocupied())|| (nb.squares[x, y].diagonalcheck_b_r&& x < 8 && y < 8 &&nb.squares[x+1, y+1].bishopc()&& nb.squares[x + 1, y + 1].pin.side!=side && king.x < x))
                {
                    bool dont = false;
                    for (int ix = x - 1, iy = y - 1; ix > 0 && iy > 0; ix--, iy--){
                        if (nb.squares[ix, iy].isocupied())
                        {
                            if(nb.squares[ix, iy].pin != king)
                            {
                                dont = true;
                            }
                            break;
                        }
                    }
                    if (!dont)
                    {
                        righthandcheck = true;
                    }
                }
                if ((x > 1 && y>1&& nb.squares[x - 1, y-1].diagonalcheck_b_l&&!nb.squares[x - 1, y - 1].isocupied()) || (nb.squares[x, y].diagonalcheck_b_l&& x > 1 && y > 1 && nb.squares[x - 1, y - 1].bishopc() && nb.squares[x - 1, y - 1].pin.side != side && king.x > x))
                {
                    bool dont = false;
                    for (int ix = x + 1, iy = y + 1; ix <9 && iy <9; ix++, iy++)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            if (nb.squares[ix, iy].pin != king)
                            {
                                dont = true;
                            }
                            break;
                        }
                    }
                    if (!dont)
                    {
                        lefthandcheck = true;
                    }
                    
                }
                if ((king.x > x && king.y>y) && righthandcheck)
                {
                    return false;//it means that there is something blocking the check
                }
                else if ((king.x < x && king.y < y) && lefthandcheck)
                {
                    return false;
                }
                if (!(righthandcheck || lefthandcheck))
                {
                    return false;//there are no checks close to the piece ##
                }
                if (righthandcheck)
                {
                    bool undercheck = true;
                    int iy = y-1;
                    for (int ix = x - 1; ix > king.x && iy>king.y; ix--,iy--)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
                if (lefthandcheck)
                {
                    bool undercheck = true;
                    int iy = y + 1;
                    for (int ix = x + 1; ix < king.x && iy<king.y; ix++,iy++)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
            }
            else
            {
                var king = nb.getblakking();
                if (king.x - x != king.y - y)
                {
                    return false;//king will never be checked ##
                }
                bool righthandcheck = false;
                bool lefthandcheck = false;
                if ((x < 8 && y < 8 && nb.squares[x + 1, y + 1].diagonalcheck_w_r&& !nb.squares[x + 1, y + 1].isocupied()) ||(nb.squares[x, y].diagonalcheck_w_r && x < 8 && y < 8 && nb.squares[x + 1, y + 1].bishopc() && nb.squares[x + 1, y + 1].pin.side != side && king.x < x))
                {
                    bool dont = false;
                    for (int ix = x - 1, iy = y - 1; ix > 0 && iy > 0; ix--, iy--)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            if (nb.squares[ix, iy].pin != king)
                            {
                                dont = true;
                            }
                            break;
                        }
                    }
                    if (!dont)
                    {
                        righthandcheck = true;
                    }
                }
                if ((x > 1 && y > 1 && nb.squares[x - 1, y - 1].diagonalcheck_w_l && !nb.squares[x - 1, y - 1].isocupied())|| (nb.squares[x, y].diagonalcheck_w_l&& x > 1 && y > 1 && nb.squares[x - 1, y - 1].bishopc() && nb.squares[x - 1, y - 1].pin.side != side && king.x > x))
                {
                    bool dont = false;
                    for (int ix = x + 1, iy = y + 1; ix < 9 && iy < 9; ix++, iy++)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            if (nb.squares[ix, iy].pin != king)
                            {
                                dont = true;
                            }
                            break;
                        }
                    }
                    if (!dont)
                    {
                        lefthandcheck = true;
                    }
                }
                if ((king.x > x && king.y > y) && righthandcheck)
                {
                    return false;//it means that there is something blocking the check
                }
                else if ((king.x < x && king.y < y) && lefthandcheck)
                {
                    return false;
                }
                if (!(righthandcheck || lefthandcheck))
                {
                    return false;//there are no checks close to the piece ##
                }
                if (righthandcheck)
                {
                    bool undercheck = true;
                    int iy = y - 1;
                    for (int ix = x - 1; ix > king.x && iy > king.y; ix--, iy--)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
                if (lefthandcheck)
                {
                    bool undercheck = true;
                    int iy = y + 1;
                    for (int ix = x + 1; ix < king.x && iy < king.y; ix++, iy++)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        public bool diagonal_right_check()
        {
            if (side == 0)
            {
                var king = nb.getwhiteking();
                if (king.x - x !=  y-king.y)
                {
                    return false;//king will never be checked ##
                }
                bool righthandcheck = false;
                bool lefthandcheck = false;
                if ((x < 8 && y >1 && nb.squares[x + 1, y - 1].diagonalcheck_b_r && !nb.squares[x + 1, y - 1].isocupied()) || (nb.squares[x, y].diagonalcheck_b_r && x < 8 && y > 1 && nb.squares[x + 1, y - 1].bishopc() && nb.squares[x + 1, y - 1].pin.side != side && king.x < x))
                {
                    bool dont = false;
                    for (int ix = x - 1, iy = y + 1; ix >0 && iy < 9; ix--, iy++)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            if (nb.squares[ix, iy].pin != king)
                            {
                                dont = true;
                            }
                            break;
                        }
                    }
                    if (!dont)
                    {
                        righthandcheck = true;
                    }
                    
                }
                if ((x > 1 && y < 8 && nb.squares[x - 1, y + 1].diagonalcheck_b_l && !nb.squares[x - 1, y + 1].isocupied())|| (nb.squares[x, y].diagonalcheck_b_l && x > 1 && y < 8 && nb.squares[x - 1, y + 1].bishopc() && nb.squares[x - 1, y + 1].pin.side != side && king.x > x))
                {
                    bool dont = false;
                    for (int ix = x + 1, iy = y - 1; ix <9 && iy >0; ix++, iy--)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            if (nb.squares[ix, iy].pin != king)
                            {
                                dont = true;
                            }
                            break;
                        }
                    }
                    if (!dont)
                    {
                        lefthandcheck = true;
                    }
                }
                if ((king.x > x && king.y > y) && righthandcheck)
                {
                    return false;//it means that there is something blocking the check
                }
                else if ((king.x < x && king.y < y) && lefthandcheck)
                {
                    return false;
                }
                if (!(righthandcheck || lefthandcheck))
                {
                    return false;//there are no checks close to the piece ##
                }
                if (righthandcheck)
                {
                    bool undercheck = true;
                    int iy = y + 1;
                    for (int ix = x - 1; ix > king.x && iy < king.y; ix--, iy++)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
                if (lefthandcheck)
                {
                    bool undercheck = true;
                    int iy = y - 1;
                    for (int ix = x + 1; ix < king.x && iy > king.y; ix++, iy--)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
            }
            else
            {
                var king = nb.getblakking();
                if (king.x - x !=  y-king.y)
                {
                    return false;//king will never be checked ##
                }
                bool righthandcheck = false;
                bool lefthandcheck = false;
                if ((x < 8 && y >1 && nb.squares[x + 1, y - 1].diagonalcheck_w_r&&!nb.squares[x + 1, y - 1].isocupied()) || (nb.squares[x, y].diagonalcheck_w_r && x < 8 && y > 1 && nb.squares[x + 1, y - 1].bishopc() && nb.squares[x + 1, y - 1].pin.side != side && king.x < x))
                {
                    bool dont = false;
                    for (int ix = x - 1, iy = y + 1; ix > 0 && iy < 9; ix--, iy++)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            if (nb.squares[ix, iy].pin != king)
                            {
                                dont = true;
                            }
                            break;
                        }
                    }
                    if (!dont)
                    {
                        righthandcheck = true;
                    }
                }
                if ((x > 1 && y <8 && nb.squares[x - 1, y + 1].diagonalcheck_w_l&&!nb.squares[x - 1, y + 1].isocupied()) || (nb.squares[x, y].diagonalcheck_w_l && x > 1 && y < 8 && nb.squares[x - 1, y + 1].bishopc() && nb.squares[x - 1, y + 1].pin.side != side && king.x > x))
                {
                    bool dont = false;
                    for (int ix = x + 1, iy = y - 1; ix < 9 && iy > 0; ix++, iy--)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            if (nb.squares[ix, iy].pin != king)
                            {
                                dont = true;
                            }
                            break;
                        }
                    }
                    if (!dont)
                    {
                        lefthandcheck = true;
                    }
                }
                if ((king.x > x && king.y > y) && righthandcheck)
                {
                    return false;//it means that there is something blocking the check
                }
                else if ((king.x < x && king.y < y) && lefthandcheck)
                {
                    return false;
                }
                if (!(righthandcheck || lefthandcheck))
                {
                    return false;//there are no checks close to the piece ##
                }
                if (righthandcheck)
                {
                    bool undercheck = true;
                    int iy = y + 1;
                    for (int ix = x - 1; ix > king.x && iy < king.y; ix--, iy++)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
                if (lefthandcheck)
                {
                    bool undercheck = true;
                    int iy = y - 1;
                    for (int ix = x + 1; ix < king.x && iy > king.y; ix++, iy--)
                    {
                        if (nb.squares[ix, iy].isocupied())
                        {
                            undercheck = false;
                            break;
                        }
                    }
                    if (undercheck)
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        public List<move> getmove()
        {
            List<move> moves = null;
            switch (mytype)
            {
                case pieceType.King:
                    return getkingmoves();
                case pieceType.Pawn:
                    moves= getpawnmove();
                    break;
                case pieceType.knight:
                    moves= getknightmoves();
                    break;
                case pieceType.Queen:
                    var l = getrookmove();
                    var ll = getbishopmove();
                    l.AddRange(ll);
                    moves = l;
                    break;
                case pieceType.Rook:
                    moves= getrookmove();
                    break;
                case pieceType.Bishop:
                    moves= getbishopmove();
                    break;
            }
            Piece king = null;
            switch (side)
            {
                case 0:
                    king = nb.getwhiteking();
                    break;
                case 1:
                    king = nb.getblakking();
                    break;
            }
            List<move> modified = new List<move>();
            foreach(var m in moves)
            {
                if (acceptance(m, king))
                {
                    modified.Add(m);
                }
            }
            return modified;
        }

    }
    public class square
    {
        public bool checkedwhite = false;
        public bool checkedblack = false;
        public bool ocupiedwhite = false;
        public bool ocupiedblack = false;
        public bool occupiedwithblackpawn = false;
        public bool occupiedwithwhitepawn = false;
        public Piece pin = Piece.getnullpiece();
        public bool rowcheck_w = false;
        public bool colcheck_w = false;
        public bool rowcheck_b = false;
        public bool colcheck_b = false;
        public bool diagonalcheck_b_l = false;
        public bool diagonalcheck_b_r = false;
        public bool diagonalcheck_w_r = false;
        public bool diagonalcheck_w_l= false;
        public bool isocupied()
        {
            return ocupiedblack || ocupiedwhite;
        }
        public bool bishopc()
        {
            switch (pin.mytype)
            {
                case pieceType.Queen:
                case pieceType.Bishop:
                    return true;
            }
            return false;
        }
        public bool rookc()
        {
            switch (pin.mytype)
            {
                case pieceType.Queen:
                case pieceType.Rook:
                    return true;
            }
            return false;
        }
        public void activatecheck(Piece p)
        {
            switch (pin.mytype)
            {
                case pieceType.King:
                    kingincheck(pin.nb,p);
                    break;
                
            }
        }
        void kingincheck(board b,Piece p)
        {
            if (p.side == pin.side)
            {
                return;
            }
            checkrecord chck = new checkrecord();
            if (pin.chck != null)
            {
                chck = pin.chck;
            }
            int[] j = { pin.x, pin.y };
            chck.kingposition = j;
            int[] j2 = { p.x, p.y };
            chck.positionofcheck = j2;
            chck.pchceck = p;
            chck.numpieces++;
            switch (p.mytype)
            {
                case pieceType.Pawn:
                case pieceType.knight:
                    chck.typecheck = 0;
                    break;
                case pieceType.Bishop:
                    chck.typecheck = 2;
                    break;
                case pieceType.Rook:
                    chck.typecheck = 1;
                    break;
                case pieceType.Queen:
                    if (p.x==pin.x || p.y == pin.y)
                    {
                        chck.typecheck = 1;
                    }
                    else
                    {
                        chck.typecheck = 2;
                    }
                    break;
            }
            chck.refresh();
            pin.chck = chck;

        }
        public void reset()
        { checkedwhite = false;
         checkedblack = false;
         ocupiedwhite = false;
         ocupiedblack = false;
         occupiedwithblackpawn = false;
         occupiedwithwhitepawn = false;
         rowcheck_w = false;
         colcheck_w = false;
        rowcheck_b = false;
         colcheck_b = false;
         diagonalcheck_b_l = false;
        diagonalcheck_b_r = false;
         diagonalcheck_w_r = false;
        diagonalcheck_w_l= false;
            pin.chck = null;
        pin = Piece.getnullpiece();

            //setme();
        }
        public void setme() {
            occupiedwithwhitepawn = false;
            occupiedwithblackpawn = false;
            ocupiedwhite = false;
            ocupiedblack = false;
            if (pin.mytype == pieceType.nullpiece)
            {

            }
            else
            {
                if (pin.side == 0)
                {
                    ocupiedwhite = true;
                    if (pin.mytype == pieceType.Pawn)
                    {
                        occupiedwithwhitepawn = true;
                    }
                }
                else
                {
                    ocupiedblack = true;
                    if (pin.mytype == pieceType.Pawn)
                    {
                        occupiedwithblackpawn = true;
                    }
                }
            }
        }
    }
    public class checkrecord
    {
        public int numpieces = 0;
        public int[] positionofcheck;
        public Piece pchceck;
        public int typecheck = 0;//0 check from a pawn or knight, 1 check from a rock,2 check from a bishop
        public int[] kingposition;
        public bool xmatch=false;
        public  bool rookmatch = false;
        public bool diagnoalright = false;
        public void refresh()
        {
            switch (typecheck)
            {
                case 1:
                    rookmatch = true;
                    if (positionofcheck[0] == kingposition[0])
                    {
                        xmatch = true;
                    }
                    break;
                case 2:
                   // if (positionofcheck[0] - kingposition[0] == positionofcheck[1] - kingposition[1])
                    {
                        diagnoalright = true;
                    }
                    break;
            }
        }
        public bool match_bishop(int[] pos)
        {
            bool fg, fg2, fg3;
            bool fg1, fg21, fg31;
            int dx = pos[0] - kingposition[0];
            int dy = pos[1] - kingposition[1];
            if (!(dx == dy || dx == -dy))
            {
                return false;
            }
            switch (diagnoalright)
            {
                case true:
                    fg = pos[0] - kingposition[0] > 0;
                    fg1 = pos[1] - kingposition[1] > 0;
                    fg2 = positionofcheck[0] - kingposition[0] > 0;
                    fg21 = positionofcheck[1] - kingposition[1] > 0;
                    fg3 = positionofcheck[0] - pos[0] > 0;
                    fg31 = positionofcheck[1] - pos[1] > 0;
                    if (fg == fg2 && fg2 == fg3 && fg21 == fg1 && fg31 == fg21)
                    {
                        return true;
                    }
                    return false;
            }
            return true;
        }
        public bool match_rook(int[] pos)
        {
            bool fg, fg2, fg3;
            switch (xmatch)
            {
                case true:
                    if (pos[0] != kingposition[0])
                    {
                        return false;
                    }
                     fg = pos[1] - kingposition[1]>0;
                     fg2 = positionofcheck[1] - kingposition[1] > 0;
                     fg3 =  positionofcheck[1]- pos[1]  > 0;
                    if (fg==fg2&&fg2==fg3)
                    {
                        return true;
                    }
                    return false;
                case false:
                    if (pos[1] != kingposition[1])
                    {
                        return false;
                    }
                    fg = pos[0] - kingposition[0] > 0;
                    fg2 = positionofcheck[0] - kingposition[0] > 0;
                    fg3 = positionofcheck[0]-pos[0]  > 0;
                    if (fg == fg2 && fg2 == fg3)
                    {
                        return true;
                    }
                    return false;
            }
            return false;
        }
    }
    public class move
    {
        public Piece part1 = null;
        public Piece part2 = null;
        public int[] position1 = null;
        public int[] position2 = null;
        public int itscastle = -1;
        public bool longmove = false;
        public bool promote = false;
        public pieceType promotion;
        public override string ToString()
        {
            string partr = part1.ToString()+" ";
            string mkv="";
            switch (position1[0])
            {
                case 1:
                    mkv = "h";
                    break;
                case 2:
                    mkv = "g";
                    break;
                case 3:
                    mkv = "f";
                    break;
                case 4:
                    mkv = "e";
                    break;
                case 5:
                    mkv = "d";
                    break;
                case 6:
                    mkv = "c";
                    break;
                case 7:
                    mkv = "b";
                    break;
                case 8:
                    mkv = "a";
                    break;
            }
            mkv += position1[1]+"";

            return partr+mkv;
        }
        public bool match_bishop()
        {
            int[] pos = { part1.x, part1.y };
            bool fg, fg2, fg3;
            bool fg1, fg21, fg31;
            int dx = pos[0] - position1[0];
            int dy = pos[1] - position1[1];
            if (!(dx == dy || dx == -dy))
            {
                return true;
            }
            switch (part1.chck.diagnoalright)
            {
                case true:
                    fg = pos[0] - position1[0] > 0;
                    fg1 = pos[1] - position1[1] > 0;
                    fg2 = part1.chck.positionofcheck[0] - position1[0] > 0;
                    fg21 = part1.chck.positionofcheck[1] - position1[1] > 0;
                    fg3 = part1.chck.positionofcheck[0] - pos[0] > 0;
                    fg31 = part1.chck.positionofcheck[1] - pos[1] > 0;
                    if (fg == fg2 && fg2 == fg3 && fg21 == fg1 && fg31 == fg21)
                    {
                        return false;
                    }
                    return true;
            }
            return true;
        }
        public bool match_rook()
        {
            if (!part1.chck.rookmatch)
            {
                return true;
            }
            int[] pos = { part1.x, part1.y };
            bool fg, fg2, fg3;

            switch (part1.chck.xmatch)
            {
                case true:
                    if (pos[0] != position1[0])
                    {
                        return true;
                    }
                    fg = pos[1] - position1[1] > 0;
                    fg2 = part1.chck.positionofcheck[1] - position1[1] > 0;
                    fg3 = part1.chck.positionofcheck[1] - pos[1] > 0;
                    if (fg == fg2 && fg2 == fg3)
                    {
                        return false;
                    }
                    return true;
                case false:
                    if (pos[1] != position1[1])
                    {
                        return true;
                    }
                    fg = pos[0] - position1[0] > 0;
                    fg2 = part1.chck.positionofcheck[0] - position1[0] > 0;
                    fg3 = part1.chck.positionofcheck[0] - pos[0] > 0;
                    if (fg == fg2 && fg2 == fg3)
                    {
                        return false;
                    }
                    return true;
            }
            return true;
        }
    }
}

