fd=open("F:\Zensar\Project\Project\s.txt",'r')
data=fd.read()
fd.close()
k=data.split("\n")
m=[]
print(k)
for i in k:
    l=i.split(",")
    m.append(l)
exampleData = m
######################Row and Column length specified######################################
print(exampleData)
row = len(exampleData)-2
col = len(exampleData[0])-2
#print('row and col lengths:',row,col)
p=[]
##################List Defining############################################################ 
Listnn = []
newListnn = []
avg = []
count = 0
Listcpy = exampleData
#print('\nOriginal List',Listcpy)
###########################################################################################
################Original List Without names and product names#######################################
for i in range(1,row+1):
    temp=[]
    for j in range(1,col+1):
        temp.append(Listcpy[i][j])
    newListnn.append(temp)
    p.append(Listcpy[i][0])
#print('New List without names and product names:',newListnn)
########################################################################################################################
#########################################avg calculation##############################################################
for i in range(1,row+1):
    sum = 0
    counter =0
    for j in range(1,col+1):
        if(float(Listcpy[i][j]) != 0.0):
            counter+=1
        sum+=float(Listcpy[i][j])
    avg.append(float(sum/counter))

#print('average',avg)
########################################################################################################################

#############################################new List With subracted Average############################################
for i in range(1,row+1):
    for j in range(1,col+1):
            if(float(Listcpy[i][j])!=0.0):
                Listcpy[i][j] = float(Listcpy[i][j]) - float(avg[i-1])
#print('\n\n')
#print('Updated List',Listcpy)
#print('\n\n')
#########################################################################################################################
################################List without names and product names With subracted Average####################################################
for i in range(1,row+1):
    temp=[]
    for j in range(1,col+1):
        temp.append(Listcpy[i][j])
    Listnn.append(temp)    
#print('List without names and product names:',Listnn)
########################################################################################################################

###############################Item-Item Similarity Matrix###############################################################
#print('col value:',col)
Ssum =0.0
num =0.0
den1 = 0.0
den2 = 0.0
a = 0
#S_items = []
S_items=[[0 for x in range(row)]for x in range(row)]
#print('S_item',S_items)
for z in range(row-1):
    a=z
    tmp = []
    while(a<row-1):
    
        tmplst1 = []
        tmplst2 = []
        rowt = 0
        colt = 0
    #for i in range(row-a):
        #print('value of j:',j)
        for j in range(col):
            num = 0.0
            den1 = 0.0
            den2 = 0.0
            tmplst1.append(Listnn[z][j])
            tmplst2.append(Listnn[a+1][j])
        rowt = z
        colt = a+1
        #print('LoopA',count)
        count+=1
        #print(tmplst1)
        #print(tmplst2)
        for i in range(len(tmplst1)):
        #print('i val:',i)
            num += float(tmplst1[i]) * float(tmplst2[i])
            den1 += float(tmplst1[i])**2
            den2 += float(tmplst2[i])**2
        if(den1!=0 and den2!=0):
            Ssum = num/(den1**0.5 * den2**0.5)
        
        else:
            Ssum = -5.0
        #print('LoopA:',count,'den1:',den1,'den2:',den2,'Sum:',Ssum,'row:',z,'col',a+1)
        S_items[z][a+1] = S_items[a+1][z] = Ssum
        for i in range(row):
            for j in range(row):
                if(i==j):
                    S_items[i][j]=1.0
                    
        #print('z',z,'a+1',a+1)        
        #S_items[z][a] = Ssum
        #S_items[a][z] = Ssum
        
        a+=1
#print('\nFinal Similarity Matrix\n\n',S_items,)        
#print('S_item',S_items)
#print('Example data',exampleData)
#print('New List without names and product names:',newListnn)
#########################################################################################################################
exampleData=m
ratings=[0 for i in range(0,row)]
j=0
sumsimilarity=0
sumofratings=[0 for i in range(0,row)]
for l in range (0,row):
    for i in range (0,row):
        sumsimilarity=sumsimilarity+abs(S_items[l][i])
    for i in range (0,row):
        sumofratings[l]+=abs(float(newListnn[i][0]))*abs(float(S_items[l][i]))
    ratings[j]=sumofratings[l]/sumsimilarity    
    sumsimilarity=0
    j=j+1
    l=l+1
#print(ratings)
for i in range(0,row):
    for j in range(1,row):
        if(float(ratings[j])>float(ratings[j-1])):
            temp=ratings[j-1]
            ratings[j-1]=ratings[j]
            ratings[j]=temp
            temp=p[j-1]
            p[j-1]=p[j]
            p[j]=temp
file=open("F:\Zensar\Project\Project\OutputPy.txt",'w')
for i in p:
    file.write("%s\n"%i)
file.close()