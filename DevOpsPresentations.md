# ClimateCtrl

Realtime data visualization of temperature and humidity data from a ESP32 in a dashboard.
- [GH Actions](https://github.com/patrikvalentiny/ClimateCtrl/actions)
- [Azure](https://portal.azure.com/#@easv.dk/resource/subscriptions/781ce0e4-ec89-4e8d-8ed2-19104bc11abc/resourceGroups/climate-ctrl/overview)
- [FireBase](https://console.firebase.google.com/u/0/)
- [Production](https://climatectrl.web.app/)
- [Staging](https://climatectrl-staging.web.app/)

### Technologies
- GitHub Actions
- Docker
- SonarCloud (Code Quality)
- Azure (Backend)
- Firebase (Frontend)
- Render (Database)

### [Pull Request](.github/workflows/build.yml)
- Runs on any pull request created / updated
- Builds the project and runs unit tests
- Runs SonarCloud analysis
- Checks database migrations

### [Staging Pull Request](.github/workflows/staging-pr.yml)
- Runs on pull requests to staging
- Builds the project and runs integrations tests

### [Staging Push](.github/workflows/staging-push.yml)
- Runs on push to staging
- Containerizes the project and pushes it to Docker Hub with the tag `staging`
- Deploys the project to staging backend on Azure
- Deploys the project to staging frontend on Firebase
- Runs database migrations
- Runs load tests

### [Main Push](.github/workflows/delivery.yml)
- Runs on push to main
- Containerizes the project and pushes it to Docker Hub with the tag `main`
- Deploys the project to production backend on Azure
- Deploys the project to production frontend on Firebase
- Runs database migrations

