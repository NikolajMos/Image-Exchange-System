package dk.itu.pervasive.imageview;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;

import android.R.bool;
import android.os.Environment;
import android.util.Base64;
import android.util.Base64InputStream;
import android.util.Log;

import itu.pervasive.json.JSONArray;
import itu.pervasive.json.JSONException;
import itu.pervasive.json.JSONObject;

public class Images {

	File small;

	Images(String ip) {
	
		String ExternalStorageDirectoryPath = Environment.getExternalStorageDirectory().getAbsolutePath();
		String small_lib = ExternalStorageDirectoryPath + "/DCIM/JSON/";
		createDirIfNotExists(small_lib);
		File small_targetDirector = new File(small_lib);
		small = new File(small_targetDirector, "jSONArray.txt");
		if(small.exists())
		{
			small.delete();
			small = new File(small_targetDirector, "jSONArray.txt");
		}
		writer("{\"src_ip\":\""+ip+"\",\"images\":[");
	}

	public JSONObject addJSONimage(File file, String operation) throws IOException {
		JSONObject image = getJsonImage(file);
		
		if (operation.equals("new"))
		{
			this.setImages(image.toString()+",");
		} else if (operation.equals("end"))
		{
			this.setImages(image.toString());
		}
		return image;
	}
	public static JSONObject getJsonImage(File file) throws IOException
	{
		File transferFile = file;
		byte[] bytearray = new byte[(int) transferFile.length()];
		FileInputStream fin = new FileInputStream(transferFile);
		BufferedInputStream bin = new BufferedInputStream(fin);
		bin.read(bytearray, 0, bytearray.length);

		String encodedImage = Base64.encodeToString(bytearray, Base64.DEFAULT);

		JSONObject image = new JSONObject();
		JSONObject image_data = new JSONObject();
		try {
			image_data.put("name", "Surface_Exchange_"+file.getName());
			image_data.put("bytes", encodedImage);
			image.put("image", image_data);

		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return image;
	}
	public static void saveJsonImage(JSONObject image,String TAG,String imageDir) throws IOException, JSONException
	{
		byte[] bytearray = Base64.decode(image.get("bytes").toString(),Base64.DEFAULT);
		String ExternalStorageDirectoryPath = Environment.getExternalStorageDirectory().getAbsolutePath();
		String JSON_lib = ExternalStorageDirectoryPath + imageDir;
		File JSON_targetDirector = new File(JSON_lib);
		File JSON_image = new File(JSON_targetDirector, "Surface_Exchange_"+image.get("name").toString());
		/*
		if(!JSON_image.exists())
		{
			
		}Log.v(TAG, "File already exist!");
		*/
		FileOutputStream fos = new FileOutputStream(JSON_image);
		BufferedOutputStream bin = new BufferedOutputStream(fos);
		bin.write(bytearray, 0, bytearray.length);

		Log.v(TAG, "File: "+JSON_image.getName()+" saved Succesfully!");
	}

	public void setImages(String image) {
		writer(image);
	}

	public File getImageCollection() throws JSONException {
		
		return small;
	}
	public void writer(String tofile)
	{
		BufferedWriter bw = null;

		try {
		    bw = new BufferedWriter(new FileWriter(small, true));
		    bw.write(tofile);
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
		/*
		FileWriter fstream;
		try {
			fstream = new FileWriter(small);

			BufferedWriter out = new BufferedWriter(fstream);
			out.write(tofile);
			// Close the output stream
			out.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}*/
	}

	
	private boolean createDirIfNotExists(String path) {
		boolean ret = true;

		File file = new File(path);
		if (!file.exists()) {
			if (!file.mkdirs()) {
				Log.v("Image Class", "Problem creating Image folder");
				ret = false;
			}
		}
		Log.v("Image Class", "Folder exists: " + String.valueOf(ret));
		return ret;
	}
}
