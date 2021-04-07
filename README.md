# TUCIL 3 IF221: Implementasi Algoritma A* untuk Menentukan Lintasan Terpendek

## Deskripsi
Program ini dibuat untuk memenuhi tugas kecil ketiga dari mata kuliah IF2211 Strategi Algoritma Semester 2 tahun 2020/2021. Program ini dibangun dengan bahasa C# menggunakan Visual Studio. Program memiliki GUI dan memiliki fitur untuk mencari rute terpendek menggunakan algoritma A*. Algoritma A* melakukan pencarian heuristik dengan mempertimbangkan nilai fungsi g(n), jarak yang sudah dilalui, dan h(n), taksiran jarak yang harus ditempuh untuk sampai tujuan. Penelurusan node diprioritaskan berdasarkan nilai g(n) dan h(n) terkecil.

## Program berjalan normal dengan spesifikasi:
* Windows 10 64-bit
* .NET Framework 4.8
* Ter-install Visual Studio Community 2019

## Cara Menggunakan
0. Jalankan setup.exe (Program seharusnya tampil dengan sendirinya setelah setup selesai) atau Fake_Book.exe pada bin/Setup.zip.
* Alternatif lain jika gagal: import seluruh project source code (pada src/Backup/A_Star.zip) ke dalam Visual Studio, lalu jalankan dengan mem-build ulang source code program.
1. Klik tombol ‘Browse’.
2. Pilih sebuah file teks yang ingin digunakan, pastikan nama file diawali 'Graph_...', lalu klik tombol ‘OK’.
3. Klik tombol 'Coords'.
4. Pilih sebuah file teks yang bersesuaian (memiliki nama akhiran yang sama) dengan file graf yang sudah dpilih sebelumnya, pastika nama file diawali 'Coords...', lalu klik tombol ‘OK’.
5. Pilih sebuah simpul pada field 'Intersection 1' dan simpul lain pada field 'Intersection 2'.
6. Opsional: beri tanda cek pada field 'Show Steps' untuk menampilkan proses penelusuran.
7. Klik tombol 'Submit'.

## Tentang Pembuat
Program ini dibuat oleh Aurelius Marcel Candra (NIM 13519198) sebagai mahasiswa semester 4 IF ITB pada April 2021. Terima kasih atas perhatiannya.