# 「Unity入門 2DアクションRPGを作って、Unityゲーム開発を始めよう」を参考に作成した見下ろし型、ARPG


## 1. ドキュメント作成者

- Nekoinu

## 2. 改訂履歴

- 1.0 
  - 作成日時 2023-09-09 19:56:12
  - 更新内容 初版作成

## 3. このドキュメントの目次
- [「Unity入門 2DアクションRPGを作って、Unityゲーム開発を始めよう」を参考に作成した見下ろし型、ARPG](#unity入門-2dアクションrpgを作ってunityゲーム開発を始めようを参考に作成した見下ろし型arpg)
  - [1. ドキュメント作成者](#1-ドキュメント作成者)
  - [2. 改訂履歴](#2-改訂履歴)
  - [3. このドキュメントの目次](#3-このドキュメントの目次)
  - [4. このドキュメントの目的](#4-このドキュメントの目的)
  - [5. UnityEditorのversion](#5-unityeditorのversion)
  - [6. 操作方法](#6-操作方法)
  - [7. 備考](#7-備考)
    - [7.1 Save方法](#71-save方法)
    - [7.2 SaveDataへのアクセス方法](#72-savedataへのアクセス方法)
  - [8. 各ファイル説明](#8-各ファイル説明)


## 4. このドキュメントの目的

開発に使用したUnityのversionや、各ファイルの役割を見返すために作成

## 5. UnityEditorのversion

2022.3.7f1

## 6. 操作方法

- Move : Arrow Keys
- Attack : Z
- Dash : X
- Status : E
- Check : A
- Save : Check the Statue(像を調べる) 


## 7. 備考

### 7.1 Save方法

- JsonUtilityとApplication.persistentDataPathを使用。

### 7.2 SaveDataへのアクセス方法

- C://User/user_name/Appdate/LocalLowの下にある、defaultcompanyやCompanyNameという名前のディレクトリを開く。その中にtest_rpg.jsonという名前で保存済み

## 8. 各ファイル説明

- 後ほど記載