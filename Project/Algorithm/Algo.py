import pandas as pd
import numpy as np
from scipy.spatial.distance import cosine
from scipy.spatial.distance import pdist, squareform

data = pd.read_csv("Sample.csv")
product_Ratings = data.drop("Name", 1)
data_ibs = pd.DataFrame(index=product_Ratings.columns,columns=product_Ratings.columns)

d=[[0 for i in range(0,len(product_Ratings.columns))]for i in range(0,len(product_Ratings.columns))]
k=[[0 for i in range(0,len(product_Ratings.columns))]for i in range(0,len(product_Ratings.columns))]
m=[0 for i in range(0,len(product_Ratings.columns))]
for i in range(0,len(data_ibs.columns)) :
    for j in range(0,len(data_ibs.columns)) :
        k[i][j]=product_Ratings.ix[0,j]
        d[i][j]=cosine(product_Ratings.ix[:,i],product_Ratings.ix[:,j])
data_neighbours = pd.DataFrame(index=data_ibs.columns,columns=range(1,6))

for i in range(0,len(data_ibs.columns)):
    data_neighbours.ix[i,:] = data_ibs.ix[0:,i].sort_values(ascending=True)[:5].index
df = data_neighbours.ix[:,2:6]

m=[0 for i in range(0,len(data.columns))]

data1 = pd.read_csv("Sample.csv",index_col = 0)
m=data1.ix[0].index
p=[0 for i in range(0,len(data.columns))]
l=0
for i in m:
    p[l]=int(i)
    l=l+1

ratings=[0 for i in range (0,len(p))]
sumofratings=[0 for i in range (0,len(p))]
sumsimilarity=0
j=0

for l in range (0,len(p)-1):
    for i in range (0,len(k)):
        if(k[0][i]!=0):
            sumsimilarity=sumsimilarity+d[l][i]
    for i in range (0,len(k)):
        sumofratings[l]+=d[j][i]*k[0][i]
    j=j+1
    if (k[0][l]!=0):
        ratings[j]=k[0][l]
    else:
        ratings[j]=sumofratings[l]/sumsimilarity
    sumsimilarity=0
del(ratings[0])
del(p[len(p)-1])
for i in range(0,len(p)-1):
    for j in range(1,len(p)):
        if(ratings[j]>ratings[j-1]):
            temp=ratings[j-1]
            ratings[j-1]=ratings[j]
            ratings[j]=temp
            temp=p[j-1]
            p[j-1]=p[j]
            p[j]=temp
print(p)
file=open("l.txt",'w')
for i in p:
    file.write("%s\n"%i)
file.close()


