using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using CA.Engine;
using CA.Gfx;

namespace CA {

class CAForm: Form {
	private Controller caController = null;
	private Timer tmr;
	private const int WIDTH = 800;
	private const int HEIGHT = 600;
	private int SP_WIDTH = 640;
	private int SP_HEIGHT = 480;
	//private int PANEL_SPACE = 8;
	private StatusBar sb;
	private string generations = "";
	
	//static FastLoop _fastLoop = new FastLoop(AnimLoop);
	
	public static void Main() {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new CAForm());
    }

	public CAForm() { // CONSTRUCTOR
		DoubleBuffered = true;
        Text = "CA 2011";
        Size = new Size(WIDTH, HEIGHT);
        ToolTip tooltip = new ToolTip();
        tooltip.SetToolTip(this, "Click or draw to add cells!");
        Bitmap bm;
        try {
		    bm = LoadBitmap("bgr.jpg");
        } catch (Exception) {
		    bm = CreateBitmap(SP_WIDTH, SP_HEIGHT);
        }
		caController = new Controller(bm, bm.Width, bm.Height); // ClientSize.Width, ClientSize.Height);
        ResizeRedraw = true;
		Graphics gf = CreateGraphics();
		gf.DrawImageUnscaled(caController.Surface, 0, 0);

		//Bitmap shade = new Bitmap(ClientSize.Width, 50);
		//Graphics g = Graphics.FromImage(shade);
		//Brush b = new SolidBrush(Color.FromArgb(20, 200, 200, 200));
		//gf.FillRectangle(b, new Rectangle(0, 0, ClientSize.Width, 50));
		//g.DrawImageUnscaled(shade, 0, 0);
		//g.Dispose();
		//Graphics gs = CreateGraphics();
		//gf.DrawImageUnscaled(shade, 0, 0);

		tmr = new Timer();
		tmr.Enabled = false;
		tmr.Interval = 50;
		tmr.Tick += new EventHandler(HandleTick);
		if (!caController.Calc.GradientTest) {
			//tmr.Start();
		}

		Panel p = new Panel();
		p.Width = ClientSize.Width - bm.Width;
		p.Dock = DockStyle.Right;
		p.Parent = this;

		Button startButton = new Button();
        startButton.Text = "Start";
        startButton.Location = new Point(p.Width - startButton.Width - 10, 20);
		startButton.Anchor = AnchorStyles.Right;
        startButton.Click += new EventHandler(OnStartClick);
		startButton.Parent = p;

		sb = new StatusBar();
		sb.Parent = this;
		sb.Text = "Generations: 0";
		caController.Calc.PropertyChanged += new PropertyChangedEventHandler(OnGenerationsChanged);

        //Controls.Add();
        CenterToScreen();
    }

	public void OnGenerationsChanged(object sender, PropertyChangedEventArgs args) {
		Calc c = (Calc)sender;
		generations = (c.Overflow?"!":"") + c.Generations.ToString();
	}
		
	protected Bitmap CreateBitmap(int width, int height) {
		Bitmap bm = new Bitmap(width, height);
		Graphics g = Graphics.FromImage(bm);
		Brush b = new SolidBrush(Color.Black); //Color.FromArgb(cDead));
		g.FillRectangle(b, new Rectangle(0, 0, width, height));
		g.Dispose();
		return bm;
	}
	
	public Bitmap LoadBitmap(string filename)
	{
		Bitmap bmp = null;
		//try {
			bmp = new Bitmap(filename);
		//} catch (Exception) {
		//	bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
		//}
		return bmp;
	}

	static void AnimLoop() {
		// Get Input
		// Process
		// Render
		System.Console.WriteLine("loop");
	}

    public void OnStartClick(object sender, EventArgs e) {
		Button b = (Button) sender;
        if (tmr.Enabled) {
			b.Text = "Start";
            tmr.Stop();
        } else {
			b.Text = "Stop";
            tmr.Start();
        }
    }

     protected override void OnMouseDown(MouseEventArgs mea) {
          if (mea.Button != MouseButtons.Left)
               return;
   
          //ptLast = new Point(mea.X, mea.Y);
		System.Console.WriteLine("x = " + mea.X.ToString() + ", y= " + mea.Y.ToString());
         caController.drawing = true;
		caController.addCells(new Point(mea.X, mea.Y));
     }
	
	protected void HandleTick (object sender, EventArgs e) {
		caController.HandleTick();
			
		Graphics gf = CreateGraphics();
		//gf.DrawImageUnscaled(blurred, 0, 0);
		gf.DrawImageUnscaled(caController.Surface, 0, 0);
		sb.Text = "Generations: " + generations;
	}
	
     protected override void OnMouseMove(MouseEventArgs mea) {
          if (!caController.drawing)
               return;
		caController.addCells(new Point(mea.X, mea.Y));
		Graphics gf = CreateGraphics();
		gf.DrawImageUnscaled(caController.Surface, 0, 0);
    }
	
     protected override void OnMouseUp(MouseEventArgs mea) {
          caController.drawing = false;
		caController.mX = -1;
		caController.mY = -1;
     }

	protected override void OnPaint(PaintEventArgs pea) {
          DoPage(pea.Graphics, ForeColor, ClientSize.Width, ClientSize.Height);
    }
	
    protected void DoPage(Graphics g, Color clr, int cx, int cy) {
		g.DrawImageUnscaled(caController.Surface, 0, 0);
          //Pen pen = new Pen(clr);
   
          //g.DrawLine(pen, 0,      0, cx - 1, cy - 1);
          //g.DrawLine(pen, cx - 1, 0, 0,      cy - 1);
		//g.DrawLine(pen, frX, frY, toX, toY);
		//frX = toX;
		//frY = toY;
    }
	
	protected void CLive() {
		
	}
		
	private static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
	{
	    Bitmap blurred = new Bitmap(image.Width, image.Height);
	 
	    // make an exact copy of the bitmap provided
	    using(Graphics graphics = Graphics.FromImage(blurred))
	        graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
	            new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
	 
	    // look at every pixel in the blur rectangle
	    for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
	    {
	        for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
	        {
	            Int32 avgR = 0, avgG = 0, avgB = 0;
	            Int32 blurPixelCount = 0;
	 
	            // average the color of the red, green and blue for each pixel in the
	            // blur size while making sure you don't go outside the image bounds
	            for (Int32 x = xx; (x < xx + blurSize && x < image.Width); x++)
	            {
	                for (Int32 y = yy; (y < yy + blurSize && y < image.Height); y++)
	                {
	                    Color pixel = blurred.GetPixel(x, y);
	 
	                    avgR += pixel.R;
	                    avgG += pixel.G;
	                    avgB += pixel.B;
	 
	                    blurPixelCount++;
	                }
	            }
	 
	            avgR = avgR / blurPixelCount;
	            avgG = avgG / blurPixelCount;
	            avgB = avgB / blurPixelCount;
	 
	            // now that we know the average for the blur size, set each pixel to that color
	            for (Int32 x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
	                for (Int32 y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
	                    blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
	        }
	    }
	 
	    return blurred;
	}
	
	private static Bitmap Blur(Bitmap image, Int32 blurSize)
	{
	    return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
	}

} // end class

} // end namespace