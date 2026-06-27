using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeList.Models
{
    public class BarcodeFolder
    {
        
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public DateTime CreatedAt { get; set; }

    }


    public enum FolderFilterType
    {
        All, // 保存していないすべての履歴表示　（todo 100件まで）
        Folder // 特定のフォルダに入っているバーコードを表示
    }

    public class FolderFilterItem
    {
        public int? FolderId { get; set; }　// nullの場合はAllを意味する（フォルダに保存をしなかった）
        public string Name { get; set; } = "";　
        public FolderFilterType Type { get; set; }
    }
}
