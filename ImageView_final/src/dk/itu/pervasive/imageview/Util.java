package dk.itu.pervasive.imageview;

import java.io.BufferedWriter;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.util.Enumeration;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Matrix;
import android.os.Environment;
import android.text.format.Formatter;
import android.util.Log;
import android.widget.Toast;

public class Util {

	public static void writeToLogFile(String input)
	{
		String ExternalStorageDirectoryPath = Environment.getExternalStorageDirectory().getAbsolutePath();
		String lib = ExternalStorageDirectoryPath + "/DCIM/LOG/";
		createDirIfNotExists(lib,"LOG Writing error");
		File log_targetDirector = new File(lib);
		File log = new File(log_targetDirector, "log.txt");
		if(log.exists())
		{
			log.delete();
			log = new File(log_targetDirector, "log.txt");
		}
		writer(input,log.getAbsolutePath());
		
	}
	public static void writer(String input,String file_absolut_path)
	{
		File file = new File(file_absolut_path);	
		BufferedWriter bw = null;

		try {
		    bw = new BufferedWriter(new FileWriter(file, true));
		    bw.write(input);
		    bw.newLine();
		    bw.flush();
		} catch (IOException ioe) {
		    ioe.printStackTrace();
		} finally { // always close the file
		    if (bw != null) {
		        try {
		            bw.close();
		        } catch (IOException ioe2) {
		            // just ignore it
		        }
		    }
		}
	}
	public static File getFile(String library,String filename)
	{
		String ExternalStorageDirectoryPath = Environment.getExternalStorageDirectory().getAbsolutePath();
		String lib = ExternalStorageDirectoryPath + library;
		File targetDirector = new File(lib);
		File[] files = targetDirector.listFiles();
		
		for (File file : files)
		{
			if(filename.equals(file.getName())){return file;}
		}
		Log.v("File Error", "File Does not exist!");
		return null;
	}
	public static boolean createDirIfNotExists(String path,String TAG) {
		boolean ret = true;

		File file = new File(path);
		if (!file.exists()) {
			if (!file.mkdirs()) {
				Log.v(TAG, "Problem creating Image folder");
				ret = false;
			}
		}
		Log.v(TAG, "Folder exists: " + String.valueOf(ret));		
		return ret;
	}
	public static void copy(String src, String dst,String TAG) throws IOException {
		InputStream in = new FileInputStream(src);
		OutputStream out = new FileOutputStream(dst);

		// Transfer bytes from in to out
		byte[] buf = new byte[2024];
		int len;
		while ((len = in.read(buf)) > 0) {
			out.write(buf, 0, len);
		}
		in.close();
		out.close();
		Log.v(TAG, "image copied:" + dst);
	}
	public static void resize(String file_path, int height,int width) throws IOException {
		
		Bitmap bmp = decodeSampledBitmapFromResource(file_path, height,width);
		ByteArrayOutputStream bos = new ByteArrayOutputStream();
		bmp.compress(Bitmap.CompressFormat.JPEG, 100, bos);
		bmp.recycle();
		byte[] bitmapdata = bos.toByteArray();
		FileOutputStream fos = new FileOutputStream(new File(file_path));
		fos.write(bitmapdata);
		fos.close();
		bos.close();
	}
	public static int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight) {
	    // Raw height and width of image
	    final int height = options.outHeight;
	    final int width = options.outWidth;
	    int inSampleSize = 1;

	    if (height > reqHeight || width > reqWidth) {

	        // Calculate ratios of height and width to requested height and width
	        final int heightRatio = Math.round((float) height / (float) reqHeight);
	        final int widthRatio = Math.round((float) width / (float) reqWidth);

	        // Choose the smallest ratio as inSampleSize value, this will guarantee
	        // a final image with both dimensions larger than or equal to the
	        // requested height and width.
	        inSampleSize = heightRatio < widthRatio ? heightRatio : widthRatio;
	    }

	    return inSampleSize;
	}
	public static Bitmap decodeSampledBitmapFromResource(String file_path, int reqWidth, int reqHeight) {

	    // First decode with inJustDecodeBounds=true to check dimensions
	    final BitmapFactory.Options options = new BitmapFactory.Options();
	    options.inJustDecodeBounds = true;
	    BitmapFactory.decodeFile(file_path, options);

	    // Calculate inSampleSize
	    options.inSampleSize = calculateInSampleSize(options, reqWidth, reqHeight);

	    // Decode bitmap with inSampleSize set
	    options.inJustDecodeBounds = false;
	    return BitmapFactory.decodeFile(file_path, options);
	}
	public static boolean FileExist(String dir,String file_name)
	{
		File dir_file = new File(dir);
		//Log.v("FILE_EXIST", "number of files: "+dir_file.getAbsolutePath());
		String[] list = dir_file.list();
		for(String list_name : list)
		{
			if (list_name.equals(file_name))
			{
				return true;
			}
		}
		return false;
	}
	public static String getLocalIpAddress() {
	    try {
	        for (Enumeration<NetworkInterface> en = NetworkInterface.getNetworkInterfaces(); en.hasMoreElements();) {
	            NetworkInterface intf = en.nextElement();
	            for (Enumeration<InetAddress> enumIpAddr = intf.getInetAddresses(); enumIpAddr.hasMoreElements();) {
	                InetAddress inetAddress = enumIpAddr.nextElement();
	                if (!inetAddress.isLoopbackAddress()) {
	                    String ip = Formatter.formatIpAddress(inetAddress.hashCode());
	                    Log.i("DEVICE IP:", "***** IP="+ ip);
	                    return ip;
	                }
	            }
	        }
	    } catch (SocketException ex) {
	        Log.e("DEVICE IP ERROR:", ex.toString());
	    }
	    return null;
	}
	/*
	public Bitmap getResizedBitmap(Bitmap bmp, double scale) {
		int newHeight = (int) (bmp.getHeight() * scale);
		int newWidth = (int) (bmp.getWidth() * scale);
		int width = bmp.getWidth();
		int height = bmp.getHeight();
		float scaleWidth = ((float) newWidth) / width;
		float scaleHeight = ((float) newHeight) / height;

		// create a matrix for the manipulation
		Matrix matrix = new Matrix();

		// resize the bit map
		matrix.postScale(scaleWidth, scaleHeight);

		// recreate the new Bitmap
		Bitmap resizedBitmap = Bitmap.createBitmap(bmp, 0, 0, width, height,
				matrix, false);
		

		return resizedBitmap;

	}*/

}
