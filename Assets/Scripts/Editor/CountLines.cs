using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CountLines: EditorWindow 
{
    StringBuilder strStats;
	Vector2 scrollPosition = Vector2.zero;
    struct File
    {
        public string   name;
        public int      nbLines;
        
        public File(string name, int nbLines)
        {
            this.name       = name;
            this.nbLines    = nbLines;
        }
    }   
    
    void OnGUI()
    {
        if (GUILayout.Button("Refresh"))
        {
            DoCountLines();
        }
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.HelpBox(strStats.ToString(),MessageType.None);
        EditorGUILayout.EndScrollView();
    }
    
    
    [MenuItem("Tools/Count Lines")]
    public static void Init()
    {
        CountLines window = EditorWindow.GetWindow<CountLines>("Count Lines");
        window.Show();
        window.Focus();
        window.DoCountLines();
    }
    
    void DoCountLines()
    {       
        string strDir = System.IO.Directory.GetCurrentDirectory();
        strDir += @"/Assets";
        int iLengthOfRootPath = strDir.Length;
        ArrayList stats = new ArrayList();  
        ProcessDirectory(stats, strDir);    
        
		Dictionary<string,int> directoryLineCount = new Dictionary<string,int>();
        int iTotalNbLines = 0;
		StringBuilder strFileStats = new StringBuilder();
        foreach(File f in stats)
		{
			int lines = f.nbLines;
			string nameFormatted = f.name.Substring(iLengthOfRootPath+1, f.name.Length-iLengthOfRootPath-1);

			// Add directory info on Dictionary
			string[] directoryNameArray = nameFormatted.Split(System.IO.Path.DirectorySeparatorChar);
			string fullDirectoryString = "";
			for(int i=0;i<directoryNameArray.Length-1;i++){ // Doesn`t counts the last one, since it`s current file name
				string directoryName = directoryNameArray[i];
				fullDirectoryString+=directoryName+System.IO.Path.DirectorySeparatorChar;
				directoryLineCount[fullDirectoryString] = directoryLineCount.ContainsKey(fullDirectoryString) ? directoryLineCount[fullDirectoryString]+lines : lines;
			}

			iTotalNbLines += lines;
			string result =	nameFormatted+ " --> " + lines + "\n";
			strFileStats.Append(result);
		}       
		strStats = new StringBuilder();
		strStats.Append("Number of Files: " + stats.Count + "\n");      
		strStats.Append("Number of Lines: " + iTotalNbLines + "\n");    
		strStats.Append("================\n");  

		// Append directory
		foreach(string key in directoryLineCount.Keys){
			strStats.Append(key+ " --> " + directoryLineCount[key] + "\n");  
			
		}
		strStats.Append("================\n");  

		strStats.Append(strFileStats);

    }
    
    static void ProcessDirectory(ArrayList stats, string dir)
    {   
        string[] strArrFiles = System.IO.Directory.GetFiles(dir, "*.cs");
        foreach (string strFileName in strArrFiles)
            ProcessFile(stats, strFileName);
        
        strArrFiles = System.IO.Directory.GetFiles(dir, "*.js");
        foreach (string strFileName in strArrFiles)
			ProcessFile(stats, strFileName);
		
		strArrFiles = System.IO.Directory.GetFiles(dir, "*.boo");
		foreach (string strFileName in strArrFiles)
			ProcessFile(stats, strFileName);
        
        string[] strArrSubDir = System.IO.Directory.GetDirectories(dir);
        foreach (string strSubDir in strArrSubDir)
            ProcessDirectory(stats, strSubDir);
    }
    
    static void ProcessFile(ArrayList stats, string filename)
    {
        System.IO.StreamReader reader = System.IO.File.OpenText(filename);
        int iLineCount = 0;
        while (reader.Peek() >= 0)
        {
            reader.ReadLine();
            ++iLineCount;
        }
        stats.Add(new File(filename, iLineCount));
        reader.Close();         
    }   
}