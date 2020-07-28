# Kurdspell
Kurdspell is a project to create a Kurdish spellchecker.

## Useful links
 - [Seminar explaining how the algorithm works](https://www.facebook.com/Ahmed.A.Qadir/videos/1216344038529674/) (In kurdish)
 - [Thesis: Spell checking algorithm for agglutinative languages “Central Kurdish as an example”](https://ieeexplore.ieee.org/document/8950517). DOI: 10.1109/IEC47844.2019.8950517
 - [Demo of MS Word integration](https://www.facebook.com/kurdspell/videos/329399627757306)
 
## Structure

- `src/Kurdspell`: This is a .NET standard library containing the algorithm. It has everything discussed in the paper.
- `src/KurdspellForWord`: This is our effort on creating a plugin for Microsoft word.
- `src/Sample3`: This is a sample application that uses the algorithm.
- `src/DictionaryEditor`: This is a little application we used to create pattern lists.
- `src/Sample3/ckb-IQ.txt`: This is a sample pattern list, however it's not 100% correct and needs a lot of cleaning up.
