import java.io.*;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.Set;

public class StorySplitter {

	public static void main(String[] args) throws IOException {
		
		//File Reader
		// =================================
		BufferedReader in = new BufferedReader(
				new InputStreamReader(new FileInputStream("C:\\Users\\GAhme\\Desktop\\Story.txt"), "UTF8"));
		String str;
		String[] eFile = new String[5];
		while ((str = in.readLine()) != null) 
			eFile = str.split(" ");
		in.close();
		// =================================

		// Remove Duplicates
		// =================================
		Set<String> mySet = new HashSet<String>(Arrays.asList(eFile));
		eFile = mySet.toArray(new String[mySet.size()]);
		// =================================
		
		// File Writer
		// =================================
		FileWriter fileWriter = new FileWriter(new File("C:\\Users\\GAhme\\Desktop\\data.txt"));
		for (int i = 0; i < eFile.length; i++)
			fileWriter.write(eFile[i] + "\n");
		fileWriter.close();
		// =================================

	}
}
