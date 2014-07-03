using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class iTunesConnector : Singleton<iTunesConnector>
{
    public string iTunesLibraryLocation = string.Empty;
    public bool IsGettingFiles = true;
    public List<DirectoryInfo> DirectoryList = new List<DirectoryInfo>();

    private void Start()
    {
        if (!string.IsNullOrEmpty(iTunesLibraryLocation))
        {
            GetFiles();
        }
    }

    private void GetFiles()
    {
        // Start with drives if you have to search the entire computer. 
        IsGettingFiles = true;
            System.IO.DirectoryInfo rootDir = new System.IO.DirectoryInfo(iTunesLibraryLocation);
            WalkDirectoryTree(rootDir);
        IsGettingFiles = false;
    }

    private void WalkDirectoryTree(System.IO.DirectoryInfo root)
    {
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;

        // First, process all the files directly under this folder 
        try
        {
            DirectoryList = new List<DirectoryInfo>(root.GetDirectories());//("*.*");
        }
        // This is thrown if even one of the files requires permissions greater 
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse. 
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
           // log.Add(e.Message);
        }

        catch (System.IO.DirectoryNotFoundException e)
        {
            //Debug.Log(e.Message);
        }

        if (files != null)
        {
            foreach (System.IO.FileInfo fi in files)
            {
                // In this example, we only access the existing FileInfo object. If we 
                // want to open, delete or modify the file, then 
                // a try-catch block is required here to handle the case 
                // where the file has been deleted since the call to TraverseTree().
                //Debug.Log(fi.FullName);
            }

            // Now find all the subdirectories under this directory.
            subDirs = root.GetDirectories();

//            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
//            {
//                // Resursive call for each subdirectory.
//                WalkDirectoryTree(dirInfo);
//            }
        }            
    }

    public List<FileInfo> GetFiles(System.IO.DirectoryInfo root)
    {
        System.IO.FileInfo[] files = null;

        try
        {
            files = root.GetFiles("*.*");
        }
        // This is thrown if even one of the files requires permissions greater 
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse.
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
           // log.Add(e.Message);
        }

        catch (System.IO.DirectoryNotFoundException e)
        {
            //Debug.Log(e.Message);
        }

        return new List<FileInfo>(files);
    }
}