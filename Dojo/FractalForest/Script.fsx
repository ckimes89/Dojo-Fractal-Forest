open System
open System.Drawing
open System.Drawing.Drawing2D
open System.Windows.Forms

// Create a form to display the graphics
let width, height = 1000, 750         
let form = new Form(Width = width, Height = height)
let box = new PictureBox(BackColor = Color.White, Dock = DockStyle.Fill)
let image = new Bitmap(width, height)
let graphics = Graphics.FromImage(image)
//The following line produces higher quality images, 
//at the expense of speed. Uncomment it if you want
//more beautiful images, even if it's slower.
//Thanks to https://twitter.com/AlexKozhemiakin for the tip!
//graphics.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.HighQuality
let brush = new SolidBrush(Color.FromArgb(0, 0, 0))

box.Image <- image
form.Controls.Add(box) 

// Compute the endpoint of a line
// starting at x, y, going at a certain angle
// for a certain length. 
let endpoint x y angle length =
    x + length * cos angle,
    y + length * sin angle

let flip x = (float)height - x

// Utility function: draw a line of given width, 
// starting from x, y
// going at a certain angle, for a certain length.
let drawLine (target : Graphics) (brush : Brush) 
             (x : float) (y : float) 
             (angle : float) (length : float) (width : float) =
    let x_end, y_end = endpoint x y angle length
    let origin = new PointF((single)x, (single)(y |> flip))
    let destination = new PointF((single)x_end, (single)(y_end |> flip))
    let pen = new Pen(brush, (single)width)
    target.DrawLine(pen, origin, destination)

let draw x y angle length width brush = 
    drawLine graphics brush x y angle length width

let drawBezier (target : Graphics) (brush : Brush) 
             (x : float) (y : float) 
             (angle : float) (angleDiff : float) (length : float) (width : float) =
    let x_end, y_end = endpoint x y (angle + angleDiff) length
    let x_ctl1, y_ctl1 = endpoint x y (angle) (length*0.8)
    let x_ctl2, y_ctl2 = endpoint x_end y_end (angle) (length*(-0.3))
    let origin = new PointF((single)x, (single)(y |> flip))
    let destination = new PointF((single)x_end, (single)(y_end |> flip))
    let ctl1 = new PointF((single)x_ctl1, (single)(y_ctl1 |> flip))
    let ctl2 = new PointF((single)x_ctl2, (single)(y_ctl2 |> flip))
    let pen = new Pen(brush, (single)width)
    target.DrawBezier(pen, origin, ctl1, ctl2, destination)

let drawB x y angle angleDiff length width brush =
    drawBezier graphics brush x y angle angleDiff length width

let pi = Math.PI

// Now... your turn to draw
// The trunk
//draw 250. 50. (pi*(0.5)) 100. 4.
//let x, y = endpoint 250. 50. (pi*(0.5)) 100.
// first and second branches
//draw x y (pi*(0.5 + 0.3)) 50. 2.
//draw x y (pi*(0.5 - 0.4)) 50. 2.


let colors = [| Color.Red; Color.Orange; Color.Yellow; Color.Green; Color.Blue; Color.Indigo; Color.Violet |]

let rand = new Random(13)

let perturb x =
    x * (rand.NextDouble() * 0.2 + 0.9)


let rec tree depth x y angle angleDiff len width =
   if depth > 0 then
       let angle = perturb angle
       let len = perturb len
       let startColor = colors.[(depth / 2 + 7) % 7]
       let endColor = colors.[((depth-1)/2 + 7) % 7]
       let x2,y2 = endpoint x y (angle+angleDiff) len
       let brush = new LinearGradientBrush(new Point(int x, int y), new Point(int x2, int y2), endColor, startColor)
       drawB x y angle angleDiff len width brush
       if rand.NextDouble() > 0.5 then 
           tree (depth-1) x2 y2 (angle+angleDiff) -0.2 (len*0.75) (width*0.9)
           tree (depth-1) x2 y2 (angle+angleDiff) +0.4 (len*0.75) (width*0.9)
       else
           tree (depth-1) x2 y2 (angle+angleDiff) +0.4 (len*0.75) (width*0.9)
           tree (depth-1) x2 y2 (angle+angleDiff) -0.2 (len*0.75) (width*0.9)
       //tree (depth-2) x2 y2 angle (perturb (len/2.0)) width




tree 15 500. 0. (pi/2.0) 0. 150. 6.

form.ShowDialog()

(* To do a nice fractal tree, using recursion is
probably a good idea. The following link might
come in handy if you have never used recursion in F#:
http://en.wikibooks.org/wiki/F_Sharp_Programming/Recursion
*)