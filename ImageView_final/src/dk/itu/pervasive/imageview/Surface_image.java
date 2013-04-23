package dk.itu.pervasive.imageview;

import java.io.File;
import java.io.Serializable;

import android.os.Environment;

public class Surface_image implements Serializable
{
	private static final long serialVersionUID = 1L;
	private String file_name;
	private File file;
	
	public Surface_image(String file_name,String dir)
	{
		String ExternalStorageDirectoryPath = Environment.getExternalStorageDirectory().getAbsolutePath();
		String small_lib = ExternalStorageDirectoryPath + dir;
		File small_targetDirector = new File(small_lib);
		
		file = new File(small_targetDirector, file_name);		
	}

}
