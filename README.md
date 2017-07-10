# pr-emergency

## これって何？

GitHub API を利用して、 open な状態で対象のユーザが Author の Pull Request のうち、７日以上経過しているものがあった場合、 Slack Incomming Webhook を利用して、通知をする Lambda Script

## 使い方

### デバッグ実行

Visual Stadio で F5

### 本実行

Visual Studio でプロジェクトを右クリック > Publish to AWS Lambda…

## 今後やりたいこと

* メッセージが固定のものが出てしまうので、利用する人がいるのであれば、メッセージをカスタマイズできるようにする
