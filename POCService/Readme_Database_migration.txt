#Master Database migration
add-migration -Context MasterDBContext  initm
update-database -Context MasterDBContext  

Shared Database migration
add-migration -Context SharedDBContext  inits
update-database -Context SharedDBContext  

#there is no Dedicated db to migrate. Dedicated db will be created on the runtime (Script will be added later).


#Manually schema generation is a mandatory task everytime when database schema is changed. The generated sql will be executed when a paid user is registered 
#alone dedicated database.
#generate script instructions:
#Schema only: entire database
#data only: none   //some basic setting tables those will come later ie. Countries, states etc.