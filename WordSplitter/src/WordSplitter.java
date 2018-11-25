import java.awt.EventQueue;

import javax.swing.JFrame;
import javax.swing.JTextField;
import javax.swing.ListSelectionModel;
import javax.swing.UIManager;
import javax.swing.JList;
import javax.swing.JScrollPane;
import javax.swing.DefaultListModel;
import javax.swing.JButton;
import java.awt.event.ActionListener;
import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.awt.event.ActionEvent;
import javax.swing.JMenuBar;
import javax.swing.JOptionPane;
import javax.swing.JMenu;
import javax.swing.event.ListSelectionListener;
import javax.swing.event.ListSelectionEvent;
import javax.swing.JLabel;
import java.awt.Font;

public class WordSplitter {

	private JFrame frmKurdishSpellChecker;
	private JTextField textField;
	private DefaultListModel wordList = new DefaultListModel();
	private JList jList = new JList(wordList);

	/**
	 * Launch the application.
	 */
	public static void main(String[] args) {
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				try {
					WordSplitter window = new WordSplitter();
					window.frmKurdishSpellChecker.setVisible(true);
					// =======================
					UIManager.setLookAndFeel("com.sun.java.swing.plaf.windows.WindowsLookAndFeel");
					// ========================
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		});
	}

	/**
	 * Create the application.
	 */
	public WordSplitter() {
		initialize();
		this.bindData();
	}

	/**
	 * Initialize the contents of the frame.
	 */
	private void initialize() {
		frmKurdishSpellChecker = new JFrame();
		frmKurdishSpellChecker.setTitle("Kurdish Spell Checker");
		frmKurdishSpellChecker.setBounds(100, 100, 351, 299);
		frmKurdishSpellChecker.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frmKurdishSpellChecker.getContentPane().setLayout(null);

		textField = new JTextField();
		textField.setBounds(10, 11, 215, 29);
		frmKurdishSpellChecker.getContentPane().add(textField);
		textField.addKeyListener(new java.awt.event.KeyAdapter() {
			public void keyReleased(java.awt.event.KeyEvent evt) {
				searchTxtKeyReleased(evt);
			}
		});
		textField.setColumns(10);

		JScrollPane scrollPane = new JScrollPane();
		scrollPane.setBounds(10, 51, 315, 196);
		frmKurdishSpellChecker.getContentPane().add(scrollPane);
		jList.setFont(new Font("Tahoma", Font.PLAIN, 14));
		scrollPane.setViewportView(jList);
		
		JLabel label = new JLabel("گەڕان لە وشە:");
		label.setFont(new Font("Tahoma", Font.PLAIN, 14));
		label.setBounds(235, 14, 90, 22);
		frmKurdishSpellChecker.getContentPane().add(label);

	}

	private void searchTxtKeyReleased(java.awt.event.KeyEvent evt) {
		searchFilter(textField.getText());
	}

	private void searchFilter(String searchTerm) {
		DefaultListModel<String> filteredItems = new DefaultListModel();
		ArrayList<String> stars = getStars();

		stars.stream().forEach((star) -> {
			String starName = star.toString().toLowerCase();
			if (starName.contains(searchTerm.toLowerCase())) {
				filteredItems.addElement(star);
			}
		});
		wordList = filteredItems;
		jList.setModel(wordList);

	}

	private ArrayList<String> getStars() {
		ArrayList<String> wordList = new ArrayList<String>();
		try {
			String sourcePath = "C:\\Users\\GAhme\\Desktop\\data.txt";
			BufferedReader in = new BufferedReader(new InputStreamReader(new FileInputStream(sourcePath), "UTF8"));
			String str;
			while ((str = in.readLine()) != null)
				wordList.add(str);
			in.close();
		} catch (Exception e) {

		}
		return wordList;
	}

	private void bindData() {
		// foreach with functinal operation
		getStars().stream().forEach((star) -> {
			wordList.addElement(star);
		});
		jList.setModel(wordList);
		jList.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
	}
}
