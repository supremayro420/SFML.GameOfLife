using System;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;
static class extensionabc
{
    static public Vector2f Center(this RectangleShape self)
    {
        return new Vector2f(self.Position.X + (self.Size.X / 2f), self.Position.Y + (self.Size.Y / 2f));
    }
    static public void SetCenter(this RectangleShape self, Vector2f pos)
    {
        self.Position = new Vector2f(pos.X + (self.Size.X / 2f), pos.Y + (self.Size.Y / 2f));
    }
}
static class stuff
{
    public static Time dt
    {
        get { return Program.dt; }
    }
    public static int UnsignedModulo(int x, int max)
    {
        if (x >= 0)
        {
            return x % max;
        }
        else
        {
            return (max - (Math.Abs(x) % max)) % max;
        }
    }
}
class table
{
    char[,] main;
    int w, h;
    public table(Vector2i size)
    {
        main = new char[size.X, size.Y];
        w = size.X; h = size.Y;
        for (int x = 0; x < main.GetLength(0) ; x++)
        {
            for(int y = 0; y < main.GetLength(1); y++)
            {
                main[x, y] = ' ';
            }
        }
    }
    public void put(int x,int y, char c)
    {
        main[stuff.UnsignedModulo(x, w), stuff.UnsignedModulo(y,h)] = c;
        new Cell(new Vector2i(stuff.UnsignedModulo(x, w), stuff.UnsignedModulo(y, h)));

    }
    public char get(int x, int y)
    {
        return main[stuff.UnsignedModulo(x, w), stuff.UnsignedModulo(y, h)];
    }
    public void advance()
    {
        int neighbors = 0;
        for (int x = 0; x < main.GetLength(0); x++)
        {
            for (int y = 0; y < main.GetLength(1); y++)
            {
                if(get(x,y) == '$')
                {
                    if (get(x + 1, y + 1) == '$') neighbors++;
                    if (get(x + 0, y + 1) == '$') neighbors++;
                    if (get(x - 1, y + 1) == '$') neighbors++;
                    if (get(x - 1, y + 0) == '$') neighbors++;
                    if (get(x + 1, y + 0) == '$') neighbors++;
                    if (get(x - 1, y - 1) == '$') neighbors++;
                    if (get(x + 0, y - 1) == '$') neighbors++;
                    if (get(x + 1, y - 1) == '$') neighbors++;

                    if (neighbors < 2) Cell.GetCell(new Vector2i(x, y)).die();
                    else if (neighbors == 2 && neighbors == 3) Cell.GetCell(new Vector2i(x, y)).alivefor++;
                    else if (neighbors > 3) Cell.GetCell(new Vector2i(x, y)).die();
                    neighbors = 0;
                }
                else if(get(x,y) == ' ')
                {
                    if (get(x + 1, y + 1) == '$') neighbors++;
                    if (get(x + 0, y + 1) == '$') neighbors++;
                    if (get(x - 1, y + 1) == '$') neighbors++;
                    if (get(x - 1, y + 0) == '$') neighbors++;
                    if (get(x + 1, y + 0) == '$') neighbors++;
                    if (get(x - 1, y - 1) == '$') neighbors++;
                    if (get(x + 0, y - 1) == '$') neighbors++;
                    if (get(x + 1, y - 1) == '$') neighbors++;

                    if (neighbors == 3)
                    {
                        new Cell(new Vector2i(x, y));
                    }
                    neighbors = 0;
                }
            }
        }
    }
}
class Cell : RectangleShape
{
    static public List<Cell> cells = new List<Cell>();
    static public int snap = 16;
    public string name = "";
    public Vector2i chunk;
    public uint alivefor = 0;
    static public Cell GetCell(Vector2i chunklocate)
    {
        for(int i = 0; i < cells.Count; i++)
        {
            if(cells[i].chunk == chunklocate)
            {
                return cells[i];
            }
        }
        return null;
    }
    public Cell(Vector2f pos, Vector2f size)
    {
        this.Position = new Vector2f(pos.X - (pos.X % snap), pos.Y - (pos.Y % snap));
        this.chunk = new Vector2i((int)Position.X / snap, (int)Position.Y / snap);
        this.Size = size;
        this.FillColor = Color.Black;
        cells.Add(this);
    }
    public Cell(Vector2i Chunk)
    {
        if (!(chunk.X == -255255255 && chunk.Y == -255255255))
        {
            this.Position = new Vector2f(chunk.X * snap, chunk.Y * snap);
            this.chunk = Chunk;
            this.Size = new Vector2f(snap, snap);
            this.FillColor = Color.Black;
            cells.Add(this);
        }
        else
        {
            this.name = "empty";
        }
    }
    public void die()
    {
        cells.Remove(this);
    }
}
class Program
{
    static public RenderWindow window = new RenderWindow(new VideoMode(600, 400), "bruhmomentum", Styles.Default);
    static public table grid = new table(new Vector2i(10, 10));
    static public View mainview = new View(new Vector2f(window.Size.X / 2f, window.Size.Y / 2f), new Vector2f(window.Size.X, window.Size.Y));
    static public Time dt = new Time();
    static public Clock deltaclock = new Clock();
    static void Main(string[] args)
    {
        grid.put(10, 10, '$');
        grid.put(11, 10, '$');
        grid.put(11, 11, '$');
        grid.put(10, 11, '$');
        window.Closed += CLOSED;
        window.KeyPressed += KEYPRESS;
        Cell.cells.RemoveAt(0);  // this cause for some reason a cell gets created even though i didnt tell it to
        while(window.IsOpen)
        {
            foreach(Cell sell in Cell.cells)
            {
                Console.Write($"{sell.Position.X}, {sell.Position.Y} ");
                window.Draw(sell);
            }
            if(Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                mainview.Move(new Vector2f(10 * dt.AsSeconds(), 0));
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                mainview.Move(new Vector2f(-10 * dt.AsSeconds(), 0));
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) 
            {
                mainview.Move(new Vector2f(0, -10 * dt.AsSeconds()));
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                mainview.Move(new Vector2f(0, 10 * dt.AsSeconds()));
            }
            window.SetView(mainview);
            Console.WriteLine($"{mainview.Center.X} | {mainview.Center.Y} | {1f / dt.AsSeconds()}\n");
            window.DispatchEvents();
            window.Display();
            window.Clear(Color.White);
            window.SetFramerateLimit(1000);
            dt = deltaclock.Restart();
        }
    }
    static void CLOSED(object a, EventArgs e)
    {
        window.Close();
    }
    static void KEYPRESS(object a, KeyEventArgs e)
    {
        if(e.Code == Keyboard.Key.Space)
        {
            grid.advance();
        }
    }
}

