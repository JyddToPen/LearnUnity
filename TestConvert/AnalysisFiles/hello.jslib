
mergeInto(LibraryManager.library, {
    CreateDataManager: function() { 
        return 0;
    },

    ReleaseDataManager: function(dataManager) { 
        
    },

    InitDataManager: function(dataManager,bufferSize,configBuffer) { 
        return 0;
    },

    DataMgrPollCallback: function(dataManager) { 
        return 0;
    },

    UnitDataManager: function(dataManager) { 
        return 0;
    },

    GetDataReader: function(dataManager) { 
        return 0;
    },

    GetDataDownloader: function(dataManager,openProgressCallBack) { 
        return 0;
    },

    GetDataQuery: function(dataManager) { 
        return 0;
    },

    GetLastDataMgrError: function(dataManager) { 
        return 0;
    },

    GetDataMgrMemorySize: function(dataManager) { 
        return 0;
    }
});
