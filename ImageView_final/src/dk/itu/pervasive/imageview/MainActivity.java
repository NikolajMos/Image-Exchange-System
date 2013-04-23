package dk.itu.pervasive.imageview;

import itu.pervasive.json.JSONException;
import itu.pervasive.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.UUID;

import android.app.Activity;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Environment;
import android.os.Looper;
import android.util.Log;
import android.widget.BaseAdapter;
import android.widget.GridView;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends Activity {

	private static final String TAG = "MainActivity";

	 
	//Ray=1, Ideos=2
	int device = 2;
	
	private static String imageDir;
	int scale_ratio;
	int grid_view;
	
	ImageAdapter myImageAdapter;
	static Images imageCollection;
	ServerSocket serverSocket;
	String surface_ip;
	String android_ip;
	String ExternalStorageDirectoryPath = Environment.getExternalStorageDirectory().getAbsolutePath();
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		if (device==1){
			imageDir="100ANDRO";
			scale_ratio=100;
			grid_view=128;
		} 
		else if (device==2){
			imageDir="Camera";
			scale_ratio=50;
			grid_view=80;
		}
		
		GridView gridview = (GridView) findViewById(R.id.gridview);
		myImageAdapter = new ImageAdapter(this,grid_view);
		gridview.setAdapter(myImageAdapter);

		Thread t0 = new Thread() {
			public void run() {
				WifiManager wifiManager = (WifiManager) getSystemService(WIFI_SERVICE);
				WifiInfo wifiInfo = wifiManager.getConnectionInfo();
				int ipAddress = wifiInfo.getIpAddress();
				android_ip = String.format("%d.%d.%d.%d",(ipAddress &0xff),(ipAddress >>8&0xff),(ipAddress >>16&0xff),(ipAddress >>24&0xff));
				MainActivity.this.runOnUiThread(new Runnable() {
				    public void run() {
				    	TextView tv = (TextView)findViewById(R.id.ip);
				    	tv.setText(android_ip);
				    }
				});
				if (android_ip == null) {
					imageCollection = new Images("undefined");
				} else
					imageCollection = new Images(android_ip);
			}
		};
		t0.start();
		

		view_refresh();
	
		Thread t = new Thread() {
			public void run() {
				try {
					ServerSocket serverSocket = new ServerSocket(15123);

					while (true) {
						
						Socket socket = serverSocket.accept();
					
						Log.v(TAG, "Accepted connection : " + socket);
						BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
						JSONObject inMsg = null;
						try {
							inMsg = new JSONObject(in.readLine());
							//Util.writeToLogFile(inMsg.toString());
						} catch (JSONException e) {
							// TODO Auto-generated catch block
							Log.v(TAG,"ANDROID RECIEVED A BAD JSON STRING");
							System.exit(0);
						}

						Log.v(TAG,"incoming message: "+ inMsg.getString("method"));

						if (inMsg.getString("method").equals("SendImages")) {
							Log.v(TAG,"method: "+ inMsg.getString("method"));
							surface_ip=inMsg.getString("ip_target");
							sendImages(inMsg.getString("ip_target"));}
						else if (inMsg.getString("method").equals("transfer")) {
							Log.v(TAG,"Surface told me to transfer image to:"+inMsg.getString("dest_ip"));
							transferImage(inMsg.getString("dest_ip"),inMsg.getString("file_name"),surface_ip,android_ip);} 
						else if (inMsg.getString("method").equals("imageExist")) {
							Log.v(TAG,"Checking if image Exist");
							boolean fileExist = Util.FileExist(ExternalStorageDirectoryPath + "/DCIM/"+imageDir+"/",inMsg.get("file_name").toString());
							if(fileExist){Log.v(TAG,"Image Exist");}
							else Log.v(TAG,"Image Does not Exist");
						}
						
					}

				} catch (IOException e) {
					Log.v(TAG,"Input Server failed to initiate");
					
				} catch (JSONException e) {
					Log.v(TAG,"JSON was unable to identify the incomin message");
				}
				
			}
		};
		t.start();
		
		Thread imageTransfer = new Thread()
		{
			public void run()
			{
				try {
					  Looper.prepare();
				ServerSocket serverSocket = new ServerSocket(15125);
				
				while (true) {
					Socket socket = serverSocket.accept();
					//socket.getLocalAddress()
					Log.v(TAG, "Accepted connection : " + socket.getLocalAddress());
					
					
					//recieves the file
					int filesize=3022386;
			        int bytesRead;
			        int currentTot = 0;
			        UUID uuid = UUID.randomUUID();
	                String randomUUIDString = uuid.toString();
	        		String lib = ExternalStorageDirectoryPath + "/DCIM/"+imageDir+"/";
	        		
	        		File targetDirector = new File(lib);
	        		File image = new File(targetDirector, randomUUIDString+".jpg");
			        byte [] bytearray  = new byte [filesize];
			        Log.v(TAG, "Recieving file...");
			        InputStream is = socket.getInputStream();
			        FileOutputStream fos = new FileOutputStream(image);
			        BufferedOutputStream bos = new BufferedOutputStream(fos);
			        bytesRead = is.read(bytearray,0,bytearray.length);
			        currentTot = bytesRead;
			 
			        do {
			           bytesRead =
			              is.read(bytearray, currentTot, (bytearray.length-currentTot));
			           if(bytesRead >= 0) currentTot += bytesRead;
			        } while(bytesRead > -1);
			 
			        bos.write(bytearray, 0 , currentTot);
			        bos.flush();
			        bos.close();
			        socket.close();
			        Log.v(TAG, "Transfer complete, new file:");
			        Log.v(TAG, randomUUIDString+".jpg");
			      
			        view_refresh();
			       
			        
				}
				} catch (IOException e) {
					/*
					MainActivity.this.runOnUiThread(new Runnable() {
					    public void run() {
					    	
					    	ImageView gv = (ImageView) findViewById(R.id.on_off);
							gv.setImageResource(R.drawable.off);
					    }
					});
					e.printStackTrace();
					*/
				}
				

				
			}
		};
		imageTransfer.start();
	}
	
	protected void OnStop() {try{serverSocket.close();} catch (IOException e) {Log.v(TAG, "SERVER SOCKET CLOSED");}}

	protected void OnDestroy() throws IOException {try {serverSocket.close();} catch (IOException e) {Log.v(TAG, "SERVER SOCKET CLOSED");}}

	/*public void refresh(View view) {view_refresh();}*/

	public void view_refresh() {
		myImageAdapter.clear();
		String targetPath = ExternalStorageDirectoryPath + "/DCIM/"+imageDir+"/";
		Toast.makeText(getApplicationContext(), targetPath, Toast.LENGTH_LONG).show();

		File targetDirector = new File(targetPath);
		File[] files = targetDirector.listFiles();

		String small_lib = ExternalStorageDirectoryPath + "/DCIM/small/";
		Util.createDirIfNotExists(small_lib,TAG);
		File small_targetDirector = new File(small_lib);

		for (File file : files) {
			File small = new File(small_targetDirector, "small_"+ file.getName());
			String[] filelist =small_targetDirector.list();
			boolean file_exist=false;
			for(int i=0;i<filelist.length;i++)
			{
				//Log.v(TAG, "file:" + filelist[i]+"=="+ "small_"+ file.getName());
				if(filelist[i].equals("small_"+ file.getName()))
				{file_exist=true;}//Log.v(TAG, "True:"+filelist[i]);}
			}
			
			//if (small.exists()){small.delete();}
			try {
				if (!file_exist) {
					Util.copy(file.getAbsolutePath(), small.getAbsolutePath(),TAG);
					
					if (file.length() == small.length()) { Util.resize(small.getAbsolutePath(), scale_ratio,scale_ratio);Log.v(TAG, "Image is being resized:" + file.getName()); }
				} else Log.v(TAG, "image already exist:" + file.getName());

			} catch (IOException e) {
				Log.v(TAG, "Failed to create and/or resize image");
			}
		}
		final File[] small_files = small_targetDirector.listFiles();

		int number_of_files=small_files.length;
		int count=1;
		for (File file : small_files) {
			Log.v(TAG, file.getName());
			try {
				if (count!=number_of_files)
				{
					imageCollection.addJSONimage(file,"new");
				} else if (count==number_of_files)
				{
					imageCollection.addJSONimage(file,"end");
				}
			} catch (IOException e) {Log.v(TAG, "Could not put to JSON string: " + file.getName());	}
			
			myImageAdapter.add(file.getAbsolutePath());
			count++;
		}
		
		MainActivity.this.runOnUiThread(new Runnable() {
		    public void run() {
		    	
		    	GridView gv = (GridView) findViewById(R.id.gridview);
				((BaseAdapter) gv.getAdapter()).notifyDataSetChanged ();
		    }
		});
		
	}

	public static void sendImages(final String ip_target) throws UnknownHostException, IOException {
		Thread t = new Thread() {
			public void run() {
				try {

					Socket socket = new Socket(ip_target, 15124);
					Log.v(TAG, "Accepted connection : " + socket);
					File file = imageCollection.getImageCollection();
					if (file.exists()) {
						
						FileReader fstream;
						OutputStream os = socket.getOutputStream();
						
						Log.v(TAG, "Sending Files...");
						ByteArrayOutputStream out = new ByteArrayOutputStream();
						try {
							fstream = new FileReader(file);
							BufferedReader in = new BufferedReader(fstream);
							String tmp = "";
							
							while((tmp=in.readLine())!=null)
							{
								tmp=tmp.replaceAll("\\\\n","");
								byte[] bytearray = tmp.getBytes();
								out.write(bytearray, 0, bytearray.length);
							}
							byte[] bytearray = "]}".getBytes();
							out.write(bytearray, 0, bytearray.length);
							os.write(out.toByteArray());
							os.flush();
							in.close();
						} catch (IOException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
						
						socket.close();
						System.out.println("File transfer complete");
					} else
						Log.v(TAG, "the Json file did not exist");

				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (JSONException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
		};
		t.start();
	}
	public static void transferImage(final String target_ip,final String file_name,final String surface_ip,final String android_ip) throws UnknownHostException, IOException, JSONException
	{
		Thread test = new Thread()
		{
			public void run(){
				try {
					final String tmp_filename=file_name.replaceAll("Surface_Exchange_small_","");
					Log.v(TAG, "Sending image:"+tmp_filename+" to ip:"+target_ip);
					Socket clientSocket = new Socket(target_ip, 15125);
					Log.v(TAG, "Accepted connection : " + clientSocket);
					File transferFile = Util.getFile("/DCIM/"+imageDir+"/",tmp_filename);
				    Log.v(TAG, transferFile.exists()+"");
					if(transferFile!=null&&transferFile.exists())
					{
							byte [] bytearray  = new byte [(int)transferFile.length()];
							FileInputStream fin = new FileInputStream(transferFile);
							BufferedInputStream bin = new BufferedInputStream(fin);
							bin.read(bytearray,0,bytearray.length);
							OutputStream os = clientSocket.getOutputStream();
							Log.v(TAG, "Sending Files...");
							os.write(bytearray,0,bytearray.length);
							os.flush();
							clientSocket.close();
							bin.close();
							Log.v(TAG, "File transfer complete");
							JSONObject iconUpdate = new JSONObject();
							iconUpdate.put("method", "transfer_ok");
							iconUpdate.put("android_ip", target_ip);
							iconUpdate.put("file_name", file_name);
							
							Socket socket = new Socket(surface_ip, 15126);
							Log.v(TAG, "Accepted connection : " + socket);
							BufferedWriter out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
							// Surface_Server.init();
							Log.v(TAG, "Sending Message to surface..");
							out.write(iconUpdate.toString());
							out.flush();
							socket.close();
							Log.v(TAG, "Surface message transmittet");
			         }
				} catch (UnknownHostException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (JSONException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
					
				/*
				final String tmp_filename=file_name.replaceAll("small_","");
				Log.v(TAG, "Sending image:"+tmp_filename+" to ip:"+target_ip);
				Socket clientSocket = new Socket(target_ip, 15125);
				
				Log.v(TAG, "Accepted connection : " + clientSocket);
				File transferFile = Util.getFile("/DCIM/"+imageDir+"/",tmp_filename);
		       Log.v(TAG, transferFile.exists()+"");
				if(transferFile!=null&&transferFile.exists())
				{
					BufferedWriter bw = null;
					JSONObject tmp =Images.getJsonImage(transferFile);
					tmp.put("method", "recieve");
					BufferedWriter out = new BufferedWriter(new OutputStreamWriter(clientSocket.getOutputStream()));
					System.out.println("Sending Files...");
					out.write(tmp.toString());
					out.flush();
					clientSocket.close();
					out.close();
					System.out.println("File transfer complete");
				}else Log.v(TAG, "Falied to find/load files.");
				} catch (UnknownHostException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (JSONException e) {
					// TODO Auto-generated catch block
					e.printStackTrace(); 
				}*/
			}
				
		};
		test.start();
		
	}
			
}
