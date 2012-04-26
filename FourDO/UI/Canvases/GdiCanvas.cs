﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FourDO.UI.Canvases
{
	internal partial class GdiCanvas : UserControl, IDisposable, ICanvas
	{
		private const InterpolationMode SMOOTH_SCALING_MODE = InterpolationMode.Low;

		private const int HIGH_RES_WIDTH = 640;
		private const int HIGH_RES_HEIGHT = 480;

		private const int LOW_RES_WIDTH = HIGH_RES_WIDTH / 2;
		private const int LOW_RES_HEIGHT = HIGH_RES_HEIGHT / 2;

		protected BitmapBunch bitmapBunch;

		// I added a frameskip to help ensure that the goofy main form controls can update. Damn windows forms!!
		private volatile int refreshReliefSkips = 0; 
		private volatile int frameSkip = 1; 
		private long frameNum = 0;

		private long scanDrawTime = 0;

		private InterpolationMode scalingMode = SMOOTH_SCALING_MODE;
		private bool renderHighResolution = false;

		private bool isGraphicsIntensive = false;

		private CropHelper cropHelper = new CropHelper();

		public event BeforeRenderEventHandler BeforeRender;

		public GdiCanvas()
		{
			InitializeComponent();
		}

		public void Initialize()
		{
			this.bitmapBunch = new BitmapBunch(HIGH_RES_WIDTH, HIGH_RES_HEIGHT, PixelFormat.Format24bppRgb);

			double maxRefreshRate = (double)Utilities.DisplayHelper.GetMaximumRefreshRate();
			this.scanDrawTime = (long)((1 / maxRefreshRate) * Utilities.PerformanceCounter.Frequency);
		}

		public bool AutoCrop { get; set; }

		public bool ImageSmoothing
		{
			get
			{
				return (this.scalingMode == SMOOTH_SCALING_MODE);
			}
			set
			{
				this.scalingMode = value ? SMOOTH_SCALING_MODE : InterpolationMode.NearestNeighbor;
				this.Invalidate();
			}
		}

		public bool RenderHighResolution 
		{ 
			get
			{
				return this.renderHighResolution;
			}
			set
			{
				this.renderHighResolution = value;
				this.bitmapBunch.ClearImages();
				this.Invalidate();
			}
		}

		public bool IsInResizeMode { get; set; }

		/// <summary>
		/// Requests the draw canvas to skip a few frames so that base UI can update.
		/// </summary>
		/// <returns></returns>
		public void RequestRefresh()
		{
			// If we're nowhere near choking the UI, screw them.
			if (this.isGraphicsIntensive == false)
				return;

			refreshReliefSkips = 5;
		}

		public void Destroy()
		{
			this.Dispose();
		}

		public Bitmap GetCurrentBitmap()
		{
			if (this.bitmapBunch == null)
				return null;
			return this.bitmapBunch.GetNextRenderBitmap().Bitmap;
		}

		private void GameCanvas_Paint(object sender, PaintEventArgs e)
		{
			long sampleBefore = Utilities.PerformanceCounter.Current;

			BitmapDefinition bitmapDefinition = this.bitmapBunch.GetNextRenderBitmap();
			Bitmap bitmapToRender = bitmapDefinition == null ? null : bitmapDefinition.Bitmap;
			if (bitmapToRender == null)
				bitmapToRender = this.bitmapBunch.GetBlackBitmap().Bitmap;

			Rectangle destRect = new Rectangle(0, 0, this.Width, this.Height);
			Graphics g = e.Graphics;
			g.InterpolationMode = this.scalingMode;

			var crop = bitmapDefinition == null ? new BitmapCrop() : bitmapDefinition.Crop;
			Rectangle sourceRect = new Rectangle(
				crop.Left,
				crop.Top,
				(this.RenderHighResolution ? HIGH_RES_WIDTH : LOW_RES_WIDTH) - crop.Left - crop.Right,
				(this.RenderHighResolution ? HIGH_RES_HEIGHT : LOW_RES_HEIGHT) - crop.Top - crop.Bottom);

			g.DrawImage(bitmapToRender, destRect, sourceRect, GraphicsUnit.Pixel);

			// If we're taking longer than half of the scan time to draw, do a frame skip.
			if ((Utilities.PerformanceCounter.Current - sampleBefore) * 2 > scanDrawTime)
			{
				this.isGraphicsIntensive = true;
				this.frameSkip = 1;
			}
			else
			{
				this.isGraphicsIntensive = false;
				this.frameSkip = 0;
			}
		}

		public void PushFrame(IntPtr currentFrame)
		{
			// Skip frames?
			this.frameNum++;
			if (this.frameNum % (this.frameSkip + 1 + refreshReliefSkips) > 0)
			{
				if (refreshReliefSkips > 0)
					refreshReliefSkips--;
				return;
			}

			/////////////// 
			// Choose the best bitmap to do a background render to
			BitmapDefinition bitmapToPrepare = this.bitmapBunch.GetNextPrepareBitmap();
			bool highResolution = this.RenderHighResolution;

			// Determine how much of the image to copy.
			int copyHeight = 0;
			int copyWidth = 0;

			if (highResolution)
			{
				copyHeight = HIGH_RES_HEIGHT;
				copyWidth = HIGH_RES_WIDTH;
			}
			else
			{
				copyHeight = LOW_RES_HEIGHT;
				copyWidth = LOW_RES_WIDTH;
			}

			// Copy!
			CanvasHelper.CopyBitmap(currentFrame, bitmapToPrepare, copyWidth, copyHeight, !highResolution, false, this.AutoCrop);

			// Consider the newly determined crop rectangle.
			if (cropHelper.ConsiderAlternateCrop(bitmapToPrepare.Crop))
				Console.WriteLine("newcrop");
			bitmapToPrepare.Crop.Mimic(cropHelper.CurrentCrop);

			// And.... we're done.
			this.bitmapBunch.SetLastPreparedBitmap(bitmapToPrepare);

			// Refresh.
			this.Invalidate();
		}
	}
}