using Microsoft.Extensions.Logging;
using POS.Lib.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace POS.Lib.Data.Database.Connect
{
    public class MsSQLEntityDatabase : MsSQLDatabase
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory loggerFactory;

        public MsSQLEntityDatabase(ILoggerFactory logger, DBMode dbMode, bool encryptedConnection) : base(logger)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<MsSQLEntityDatabase>();

            if (dbMode.Equals(DBMode.Entity))
            {
                base.CreateDBSets(dbMode, encryptedConnection);
            }
        }

        public MsSQLEntityDatabase(ILoggerFactory logger, DBMode dbMode, int portalId, bool encryptedConnection) : base(logger)
        {
            loggerFactory = logger;
            _logger = logger.CreateLogger<MsSQLEntityDatabase>();


            if (dbMode.Equals(DBMode.Entity))
            {
                base.CreateDBSets(dbMode, portalId, encryptedConnection);
            }
        }

        //This should be customized..
        public override DbContext CreateContext(PersistanceContextType contextType)
        {

            DbContext context = null;
            switch (contextType)
            {
                //case PersistanceContextType.ExternalSurvey:
                //    context = new ExternalSurveyContext(base.msSqlCon); break;
                //case PersistanceContextType.Qualtrics:
                //    context = new QualtricsContext(base.msSqlCon); break;
            }
            return context;
        }

        public override DbContext CreateEEContext(DBMode dbMode, PersistanceContextType contextType)
        {
            base.GetEEConnectionString(contextType, dbMode);


            DbContext context = null;
            switch (contextType)
            {
                //case PersistanceContextType.AutomatedSampling:
                //    context = new AutomatedSamplingEntities(base.conString); break;
                //case PersistanceContextType.QuestionBuilderFrameLibrary:
                //    context = new QuestionBuilderFrameLibraryEntities(base.conString); break;
            }
            return context;
        }

        public override DbContext CreateEEContext(DBMode dbMode, int portalId, PersistanceContextType contextType)
        {
            base.GetEEConnectionString(contextType, dbMode, portalId);

            //CustomInformation cusInfo = new CustomInformation();
            //cusInfo.setInfoField("conString : " + base.conString);
            //sci.customInfor(_logDetails, cusInfo);

            DbContext context = null;
            switch (contextType)
            {
                //case PersistanceContextType.AutomatedSampling:
                //    context = new AutomatedSamplingEntities(base.conString); break;
                //case PersistanceContextType.QuestionBuilderFrameLibrary:
                //    context = new QuestionBuilderFrameLibraryEntities(base.conString); break;
            }
            return context;
        }
    }
}
