# GT.RText

Supports editing RT03, RT04, RT05 and 50TR files. 

RT05 and 50TR file support is only partial. Key algorithm is still unsolved so the current method works with brute forced keys up to X number of bytes. If the file has string longer than this, it can't be decrypted. And if you want to add a string longer than this, it won't work either. Marking the file as unobfuscated (not yet supported) could solve the last issue but won't work on the first issue.

![Preview](https://repository-images.githubusercontent.com/284490993/b578fa00-d4f2-11ea-93f0-e4cbfbbdfadd)
