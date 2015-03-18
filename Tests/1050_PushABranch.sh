#Make some changes to each repo and push the changes
cd RepoA
git checkout -f -B MyTestBranch
git push --progress  "origin" MyTestBranch:MyTestBranch

read -p "paused"

