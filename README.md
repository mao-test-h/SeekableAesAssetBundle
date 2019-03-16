# SeekableAesAssetBundle

暗号化済みAssetBundleをLoadFromStreamで復号しつつロードするテスト

当初こちらについての技術解説記事を別途用意する予定ではあったが、偶然にも全く同じ内容となる想定の記事がテラシュールブログさんより同日に先行して公開されたので具体的な概要については以下を参照。(投げ)

- [【Unity】暗号化したAssetBundleはLoadFromStreamでロードすればメモリに優しい](http://tsubakit1.hateblo.jp/entry/2019/03/16/162138)
 
ちなみに雑な纏めではあるが、検証結果については以下のwikiページに纏めている。
 
- **[検証結果](https://github.com/mao-test-h/SeekableAesAssetBundle/wiki/%E6%A4%9C%E8%A8%BC%E7%B5%90%E6%9E%9C)**
 



# ▽ 概要とか

一応簡単に概要を纏めておく。

- 発端は[こちらのスライド](https://www.slideshare.net/dena_tech/unity-20182019denaunity-dena-techcon-2019)にある[こちらのページの内容](https://image.slidesharecdn.com/techcon2019harutootake-190218063853/95/unity-20182019denaunity-dena-techcon-2019-80-638.jpg?cb=1550471988)
    - **LoadFromStreamを使うとStreamからAssetBundleを読み込める**
        - ざっくりとした条件としては、シーク可能なStreamを与える必要がある。
    - **ランダムアクセス可能な暗号をStreamとして実装することで、暗号化されたAssetBundleをメモリに全展開すること無く読める**
        - 大半の暗号化用のStreamの実装はランダムアクセス不能なので注意
        - AssetBundleのサイズが大きいと重くなるので、できるだけ小さくする
            - アセットのサイズだけでなくAssetBundleのサイズにも比例してしまう
 
 上記の内容については以下のように判断した。

- **シーク可能なStreamについて**
    - 恐らくは`Stream.CanSeek`がtrueを返す必要がある。
- **ランダムアクセス可能な暗号について**
    - そもそもの前提としてどうしてこうなってるかと言うと、**標準で実装されている[CryptoStream](https://docs.microsoft.com/ja-jp/dotnet/api/system.security.cryptography.cryptostream?view=netframework-4.7.2)の`CanSeek`が必ずfalse返すのでLoadFromStreamで使えない。**
    - その為に自前で暗号化Streamを実装する必要が出てきた。

 
    
# ▽ 参考リンク/その他メモ

## ▼ Wiki

- [検証結果](https://github.com/mao-test-h/SeekableAesAssetBundle/wiki/%E6%A4%9C%E8%A8%BC%E7%B5%90%E6%9E%9C)
    - 雑に纏めた検証結果
- [LoadFromStreamについて](https://github.com/mao-test-h/SeekableAesAssetBundle/wiki/LoadFromStream%E3%81%AB%E3%81%A4%E3%81%84%E3%81%A6)
    - `AssetBundle.LoadFromStream`について色々メモ
- [暗号化学び直し](https://github.com/mao-test-h/SeekableAesAssetBundle/wiki/%E6%9A%97%E5%8F%B7%E5%8C%96%E5%AD%A6%E3%81%B3%E7%9B%B4%E3%81%97)
    - そもそもとして暗号化に関する知識が全然なかったので学び直すところから始めていた。こちらはその際のメモ。
    
## ▼ 参考リンク

- **DeNA TechConスライド**
    - [Unity 2018-2019を見据えたDeNAのUnity開発のこれから [DeNA TechCon 2019]](https://www.slideshare.net/dena_tech/unity-20182019denaunity-dena-techcon-2019)
    - [※LoadFromStreamのページ](https://image.slidesharecdn.com/techcon2019harutootake-190218063853/95/unity-20182019denaunity-dena-techcon-2019-80-638.jpg?cb=1550471988)
- [【Unity】暗号化したAssetBundleはLoadFromStreamでロードすればメモリに優しい](http://tsubakit1.hateblo.jp/entry/2019/03/16/162138)
