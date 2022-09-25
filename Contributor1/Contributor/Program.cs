// See https://aka.ms/new-console-template for more information
using System.Drawing;
using System.Drawing.Imaging;

const int CellSize = 14;
const int CellRadius = 4;
const int CellMargin = 4;
int OuterMargin = CellMargin * 2;

const int CellMaxGray = 232;
var bgColor = Color.FromArgb(248, 248, 248);
var cellColor = Color.FromArgb(228, 228, 228);

#if DEBUG
var targetPath = "Output.png";
using var source = new Bitmap(24, 16);
using var sg = Graphics.FromImage(source);
sg.FillRectangle(Brushes.Green, 0, 0, 24, 16);
#else
if (args.Length == 0)
{
	Console.WriteLine("Input the file path of the source image.");
	return;
}
var sourcePath = args[0];
var targetPath = sourcePath + ".out.png";
using var source = new Bitmap(sourcePath);
#endif

var sw = source.Width;
var sh = source.Height;

var w = GetSize(sw) + OuterMargin * 2;
var h = GetSize(sh) + OuterMargin * 2;
var size = Math.Max(w, h);

var x0 = (size - w) / 2 + OuterMargin;
var y0 = (size - h) / 2 + OuterMargin;

using var image = new Bitmap(size, size);
using var g = Graphics.FromImage(image);
using var bgBrush = new SolidBrush(bgColor);

g.FillRectangle(bgBrush, 0, 0, size, size);
g.TranslateTransform(x0, y0);

for (int x = 0; x < sw; x++)
{
	for (int y = 0; y < sh; y++)
	{
		var c = source.GetPixel(x, y);
		c = Convert(c);
		using var brush = new SolidBrush(c);
		DrawSquare(brush, GetSize(x), GetSize(y), CellSize, CellRadius);
	}
}

image.Save(targetPath, ImageFormat.Png);

int GetSize(int cellsCount) => CellSize * cellsCount + CellMargin * (cellsCount + 1);

Color Convert(Color c)
{
	return c.R > CellMaxGray && c.B > CellMaxGray && c.G > CellMaxGray ? cellColor : c;
}

void DrawSquare(Brush brush, int x, int y, int size, int radius)
{
	var diameter = radius << 1;
	var inSize = size - diameter;
	g.FillRectangle(brush, x + radius, y + radius, inSize, inSize);
	g.FillRectangle(brush, x + radius, y, inSize, radius);
	g.FillRectangle(brush, x + radius, y + radius + inSize, inSize, radius);
	g.FillRectangle(brush, x, y + radius, radius, inSize);
	g.FillRectangle(brush, x + radius + inSize, y + radius, radius, inSize);
	g.FillPie(brush, x + inSize, y + inSize, diameter, diameter, 0, 90);
	g.FillPie(brush, x, y + inSize, diameter, diameter, 90, 90);
	g.FillPie(brush, x, y, diameter, diameter, 180, 90);
	g.FillPie(brush, x + inSize, y, diameter, diameter, 270, 90);
}
