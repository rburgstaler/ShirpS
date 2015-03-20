#Make some changes to each repo and push the changes
cd RepoA
git checkout -f -B master

#Create an annotated tag
git tag -a v1.0.annotated -m 'my version 1.0 annotted'
git push --progress  "origin" master:master

#Add a commit
echo "A" >> File1.txt
echo "B" >> File1.txt
git add File1.txt
git commit -m "This is a commit"

#Create a lightweight tag
git tag v1.0.lightweight
git push --tags --progress  "origin" master:master

read -p "paused"

