# 9cc-dotnet
[低レイヤを知りたい人のためのCコンパイラ作成入門](https://www.sigbus.info/compilerbook#)を参考に作っているLinux向けCコンパイラ

アセンブリを出力します。

Ubuntu上の.Net5.0でC#を使って開発しています。  
テストの動作環境も上記と同じです。

## 実行
```
dotnet run --project 9cc "2+3-1"
```

## テスト
```
dotnet test
```

## できること
- 括弧付きの四則演算