import java.io.*;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.Set;

public class StorySplitter {

	public static void main(String[] args) throws IOException {

		// File Reader
		String sourcePath = "C:\\Users\\GAhme\\Desktop\\Story.txt";
		String desticationPath = "C:\\Users\\GAhme\\Desktop\\data.txt";
		BufferedReader in = new BufferedReader(new InputStreamReader(new FileInputStream(sourcePath), "UTF8"));
		String str;
		int index = 0;
		String[] eFile = new String[100];
		while ((str = in.readLine()) != null)
			eFile[index++] = str;
		in.close();

		ArrayList<String> array = new ArrayList<>();
		for (int i = 0; i < eFile.length; i++) {
			String split = eFile[i];
			if (split == null)
				break;
			else {
				String check[] = split.split(" ");
				for (int j = 0; j < check.length; j++)
					array.add(check[j]);
			}

		}
		// Remove Duplicate Characters
		String[] arr = array.toArray(new String[array.size()]);
		for (int i = 0; i < arr.length; i++) {
			arr[i] = arr[i].replace(".", "");
			arr[i] = arr[i].replace("،", "");
			arr[i] = arr[i].replace(":", "");
			arr[i] = arr[i].replace("!", "");
			arr[i] = arr[i].replace("؟", "");
			arr[i] = arr[i].replace("(", "");
			arr[i] = arr[i].replace(")", "");
			arr[i] = arr[i].replace("-", "");
			arr[i] = arr[i].replace("?", "");

		}
		// Remove Duplicate Words
		Set<String> mySet = new HashSet<String>(Arrays.asList(arr));
		arr = mySet.toArray(new String[mySet.size()]);

		// File Writer
		FileWriter fileWriter = new FileWriter(new File(desticationPath));
		for (int i = 0; i < arr.length; i++)
			fileWriter.write(arr[i] + "\n");
		fileWriter.close();

	}
}
