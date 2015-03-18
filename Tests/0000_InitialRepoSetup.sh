#Sample script that will make RepoA a submodule of RepoB
set -o verbose

#Create RepoA as a test repository
rm -rf RepoA
rm -rf RepoA.git

#Create the server repositories
mkdir RepoA.git
cd ./RepoA.git
git init --bare
cd ..

#Create the client "working" repositories that is a clone of the server repo
git clone ./RepoA.git RepoA 

#Make some changes to each repo and push the changes
cd RepoA
echo "A" >> File1.txt
echo "B" >> File1.txt
git add File1.txt
git commit -m "This is a commit to RepoA"
git push --progress  "origin" master:master
cd ..

read -p "paused"

