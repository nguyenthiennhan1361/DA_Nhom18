using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

class Program
{
    // Stop words đặt ở đây, chỉ tạo 1 lần
    static readonly HashSet<string> stopWords = new HashSet<string>
    {
        "cái","và","là","có","trong","để","một","an","cho","với","vì",
        "nơi","của","đã","những","các","nhưng","khi","cho","về","thì",
        "ra","vào","này","đó","từ","bên","năm","gồm","hơn","đến","đầu",
        "phía","cũng","ngày","nay","nước","giữa","hai","ba","lần","lượt"
    };

    static string chuanHoa(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = s.ToLowerInvariant().Trim();
        // chuẩn hóa Unicode thành NFC
        s = s.Normalize(NormalizationForm.FormC);
        return s;
    }

    static bool laStopWord(string tu)
    {
        return stopWords.Contains(tu);
    }

    // Đọc file và đếm từ — dùng regex để lấy đúng "từ" (chuỗi chữ)
        static Dictionary<string, int> demTanSuatTu(string fileVao)
        {
            var danhSachTu = new Dictionary<string, int>();
            var lines = File.ReadAllLines(fileVao, Encoding.UTF8);

            foreach (var line in lines)
            {
                // Lấy tất cả token chỉ gồm chữ (bao gồm tiếng Việt) bằng regex
                foreach (Match m in Regex.Matches(line, @"\p{L}+"))
                {
                    string word = chuanHoa(m.Value);
                    if (string.IsNullOrEmpty(word)) continue;
                    if (laStopWord(word)) continue;

                    if (danhSachTu.ContainsKey(word)) danhSachTu[word]++;
                    else danhSachTu[word] = 1;
                }
            }

            return danhSachTu;
        }

    static List<KeyValuePair<string, int>> timNFromHeap(Dictionary<string, int> danhSachTu, int N)
    {
        var cmp = Comparer<KeyValuePair<string, int>>.Create((x, y) =>
        {
            int c = x.Value.CompareTo(y.Value);
            if (c == 0) c = x.Key.CompareTo(y.Key);
            return c;
        });

        var minHeap = new SortedSet<KeyValuePair<string, int>>(cmp);

        foreach (var kv in danhSachTu)
        {
            minHeap.Add(kv);
            if (minHeap.Count > N)
            {
                minHeap.Remove(minHeap.Min);
            }
        }

        return minHeap.Reverse().ToList();
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string fileVao = @"C:\nguyenthiennhan\phan_tich_van_ban\van_ban.txt";
        string fileKetQua = @"C:\nguyenthiennhan\phan_tich_van_ban\ketqua.txt";

        var danhSachTu = demTanSuatTu(fileVao);
        int N = 10;
        var topNWords = timNFromHeap(danhSachTu, N);

        using (var writer = new StreamWriter(fileKetQua, false, Encoding.UTF8))
        {
            writer.WriteLine($"Top {N} từ phổ biến nhất:");
            foreach (var w in topNWords)
                writer.WriteLine($"{w.Key}: {w.Value} lần");
        }

        Console.WriteLine("Kết quả đã được lưu vào ketqua.txt");
    }
}
