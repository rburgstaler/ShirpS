#Make a bunch of commits and push
cd RepoA
git config user.email "jdoe@myemail.com"
git config user.name "John Doe"

echo "1" >> File1.txt
git add File1.txt
git commit -m "This is a commit to RepoA 1"

echo "2" >> File1.txt
git add File1.txt
git commit -m "This is a commit to RepoA 2"

echo "3" >> File1.txt
git add File1.txt
git commit -m "This is a commit to RepoA 3"

echo "4" >> File1.txt
git add File1.txt
git commit -m "This is a commit to RepoA 4"












git push --progress  "origin" master:master

read -p "paused"

