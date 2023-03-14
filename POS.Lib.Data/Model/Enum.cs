﻿using System;
using System.Collections.Generic;
using System.Text;

namespace POS.Lib.Data.Model
{
    public enum DBMode
    {
        Portal,
        All,
        Entity,//code first
        DBRead,
        Hangfire,
        Meta,
        WAG,
        TransactionTracking,
        DB1,
        DB2,
        DB3,
        DB4,
        DB5,
        DB1AndDB2,
        DB1AndDB3,
        DB2AndDB3,
        EEntity//db first
    }
    public enum DBType
    {
        MsSQL,
        MySQL,
        Oracle,
        Access,
        MongoDB
    }
    public enum Execution
    {
        Single,
        Multiple
    }
    public enum PersistanceContextType
    {
        ExternalSurvey = 1,
        Qualtrics = 2,
        AutomatedSampling = 3,
        QuestionBuilderFrameLibrary = 4

    }

    public enum DmlType
    {
        Select = 0,
        Insert = 1,
        Update = 2,
        Delete = 3,
        Create = 4,
        Truncate = 5
    }
    public enum DataType
    {
        Binary = 0, 
        Bit = 1, 
        VarChar = 2, 
        DateTime = 3, 
        Decimal = 4, 
        Double = 5, 
        Float = 6, 
        Int = 7, 
        BigInt = 8, 
        Image = 9, 
        NVarChar = 10
    }
    public enum Timeout
    {
        Yes,
        No
    }

    public enum TransactionTrackingEntry
    {
        CreateSurvey = 1,
        UpdateSurvey = 2,
        SaveBucketDetails = 3,
        CreatePanelist = 4,
        AddPanelistNotes = 5,
        UpdatePanelist = 6,
        DeletePanelist = 7,
        AddUsersToDnnGroup = 8,
        RemoveUsersFromDnnGroup = 9,
        SaveWelcomeEmails = 10,
        ReActivateUsers = 11,
        SavePanelistFilters = 12,
        ExportPanelist = 13,
        ImportPanelist = 14,
        SiteEmail = 15,
        AddBulkAwards = 16,
        ClearPanelistAnswers = 17,
        ReservePanelist = 18,
        PreRegistrationImport = 19,
        AddNewPreRegistrationPanelistTable = 20,
        DeletePreRegistration = 21,
        SaveCalculatedQuestionVariableData = 22,
        SaveCalculatedQuestionVariableCaptionData = 23,
        DeleteCalculatedResponseVariableData = 24,
        SaveCalculatedResponseData = 25,
        SaveCalculatedAdditionalData = 26,
        SaveCalculatedAdditionalExpressionData = 27,
        EditCalculatedQuestionVariableData = 28,
        EidtCalculatedQuestionVariableCaptionData = 29,
        DeleteCalculatedResponseData = 30,
        DeleteCalculatedData = 31,
        ImportVariable = 32,
        SaveSuppression = 33,
        DeleteSuppression = 34,
        WhiteListSuppression = 35,
        AddPanelistToSuppressionList = 36,
        DeletePanelistFromSuppressionList = 37,
        WhiteListPanelistFromSuppressionList = 38,
        CreateQues = 39,
        SaveDNNRole = 40,
        EditDNNRole = 41,
        DeleteDNNRole = 42,
        SaveSocialGroup = 43,
        EditSocialGroup = 44,
        DeleteSocialGroup = 45,
        SaveDNNRoleSampling = 46,
        EditDNNRoleSampling = 47,
        RemoveDNNRoleSamplingExpressions = 48,
        RemoveDNNRoleSamplingBuckets = 49,
        SaveTestTable = 50,
        SaveTestTableLookupData = 51,
        SaveTestUserInTestTable = 52,
        UpdateTestUserInTestTable = 53,
        DeleteTestUserInTestTable = 54,
        InsertPanelParameters = 55,
        UpdateExcludeProjects = 56,
        InsertSchedulerInterval = 57,
        DeleteEmailTemplate = 58,
        InsertPanelParameterFromMailAddress = 59,
        InsertQuickPollPages = 60,
        InsertPanelParametersRollGroups = 61,
        UpdatePointRedeemStatus = 62,
        FreezPoints = 63,
        DeleteGamificationPointsConfig = 64,
        DeleteParticipantBadgesConfig = 65,
        UpdateGamificationConfig = 66,
        UpdateParticipantBadgesConfig = 67,
        InsertGamificationRoles = 68,
        DeleteGamificationRole = 69,
        EditFinancialConfig = 70,
        InsertFinancialConfig = 71,
        SavePointsByPanelist = 72,
        SavePanelistAwardPoints = 73,
        InsertQueryDetails = 74,
        InsertQueryAdditionalDetails = 75,
        DeleteBucketDetails = 76,
        DeleteBucket = 77,
        ExportBuckets = 78,
        UpdateReservedDetails = 79,
        UpdateReservedBuckets = 80,
        AddListPanelist = 81,
        SaveAddListData = 82,
        InsertAddListVariables = 83,
        SaveEmailTemplate = 84,
        UpdateEmailTemplate = 85,
        PreLaunch = 86,
        SoftLaunch = 87,
        FullLaunch = 88,
        Reminder1 = 89,
        Reminder2 = 90,
        PausedReminder = 91,
        SaveNotificationTemplate = 92,
        SMSSend = 93,
        WhatsAppSend = 94,
        LinkPreRegistrationTable = 95,
        LinkAskiaProject = 96,
        UpdateProjectLinkStatus = 97,
        SaveQuickPollQuestion = 98,
        SaveQuickPollAnswer = 99,
        UpdateDnnPageToAssign = 100,
        UpdateQuickPollQuestion = 101,
        UpdateQuickPollAnswer = 102,
        DeleteQuickPollAnswer = 103,
        GroupTalkSendImmediate = 104,
        SaveGroupTalkQuestions = 105,
        SaveSmartSurveyQuestion = 106,
        SaveSmartSurveyQuestionCaption = 107,
        AddImageResponse = 108,
        AddDualCaptionResponse = 109,
        AddSingleResponseCaption = 110,
        UpdateImageResponse = 111,
        UpdateDualCaptionResponse = 112,
        UpdateSingleCaptionResponse = 113,
        CloseProject = 114,
        ReOpenProject = 115,
        ReOpenDeletedProject = 116,
        DeleteProject = 117,
        UpdateQueueToCancelByProject = 118,
        PauseProject = 119,
        ResumeProject = 120,
        PublishQuickPollToDNN = 121,
        UnPublishQuickPollFromDNN = 122,
        AddProjectToGroup = 123,
        RemoveProjectFromGroup = 124,
        AddProjectToView = 125,
        ReAddProjectToView = 126,
        RemoveProjectFromView = 127,
        InsertParticipantBadgesConfig = 128,
        InsertGamificationConfig = 129,
        MobileQuestionRouting = 130,
        ExportPanelists = 131,
        PreRegistrationUpdate = 132,
        SaveSmartDiaryDetails = 133,
        RemoveSmartDiaryDetails = 134,
        RemoveOlderMainProjectFromSubProject = 135,
        AddSubProjectToMainProject = 136,
        ReorderSequenceForRemoval = 137,
        ReorderSequence = 138,
        AddDistributionStatusToSequence = 139,
        CopyBucketPanelists = 140,
        CopyBucketDetails = 141,
        SuppressSuppressionBucket = 142,
        RemoveProfileValueSamplingExpressions = 143,
        RemoveProfileValueSamplingBuckets = 144,
        AddQuickPollDistribution = 145,
        UpdatePanelistProjectPosition = 146,
        UpdateParticipantStatus = 147,
        DeleteFlag = 148,
        EnableEmailSentFromSheduler = 149,
        EnableEmailSentIgnoreAtSent = 150,
        AssignRoles = 151,
        RemoveRoles = 152,
        BulkUnsubscribe = 153,
        IndividualPanelistUnsubscribe = 154,
        Recalculate = 155,
        Editcalculate = 156,
        ReactivateUnsubscribePanelist = 157,
        ImportResponses = 158,
        DeleteImportedData = 159,
        CreatePortalRole = 160,
        AddRoleToGroup = 161,
        CreateModeratorGroup = 162,
        EditModeratorGroup = 163,
        DeleteModeratorGroup = 164,
        CreateAssignOnResponse = 165,
        EditAssignOnResponse = 166,
        DeleteAssignOnResponse = 167,
        CreateFlagAssignment = 168,
        EditFlagAssignment = 169,
        RemoveFlagAssignment = 170,
        CreateProfileValue = 171,
        EditProfileValue = 172,
        RemoveProfileValue = 173,
        CreateFullfillment = 174,
        EditFullfillment = 175,
        RemoveFullfillment = 176,
        AddFlag = 177,
        AddIncentive = 178,
        EditIncentive = 179,
        RemoveIncentive = 180,
        SaveQueryData = 181,
        SaveAddListBucket = 182,
        ReActivateDeletedUsers = 183,
        RemoveBeacon = 184,
        EditBeacon = 185,
        AddBeacon = 186,
        FilterReminders1 = 187,
        DeletePreRegPanelist = 188,
        UpdateAnalysisQuestion = 189,
        AddAnalysisQuestion = 190,
        DeleteDashbordSampling = 191,
        Updatedashboard = 192,
        EnablePaymentSync = 193,
        EnableUserSync = 194,
        ResendVouchers = 195,
        DeleteQPPage = 198,
        AddQPPage = 199,
        UpdateResendVoucherTraking = 196,
        Reminder3 = 197,
        Reminder4 = 200,
        Reminder5 = 201,
        OpenPendingProject = 202,
        TrackingEnableDisableEmailAppDNN = 203,
        PanelistFlagBulkInsert = 204,
        PanelistFlagIndividual = 205,
        CsvEncryptDecrypt = 206,
        UnlinkProjects = 207,
        SubCommunityDnnRoles = 208,
        SubCommunityObservableBucket = 209,
        InsertTargetedPanelistDetails = 210,
        UpdateTargetedPanelistDetails = 211,
        ChangeQuestionCustomizedCaption = 212,
        EmailTemplateAddMoreLanguages = 213,
        UpdateGeneralVariables = 214,
        InsertSubCommunityRouting = 215,
        UpdateSubCommunityRouting = 216,
        AddDashBoard = 217,
        AddDashboardGroupTalkFilters = 218,
        EditQPPage = 219,
        EnableDisableEmailFromPanelParameters = 220,
        AddPanelistsAuthRequried = 221,
        UpdatePanelistsAuthRequried = 222,
        ChatDnnRoles = 223,
        SaveP1JournalQuestion = 224,
        UpdateP1JournalQuestion = 225,
        UpdateP1JournalQuestionImage = 226,
        CreateP1JournalObserverGroup = 227,
        AssignPanelistsToSocialModeratorGroup = 228,
        RemovePanelistsFromSocialModeratorGroup = 229,
        AddJournalDisplayGroup = 230,
        InsertPanelParametersDisplayGroups = 231,
        InsertQuickPollSequence = 232,
        UpdateQuickPollSequence = 233,
        CreateSocailModeratorGroup = 234,
        EditSocialModeratorGroup = 235,
        DeleteSocialModeratorGroup = 236,
        ScheduleBucketCancel = 237,
        ScheduleBucketReSend = 238,
        DataManageCreateVariables = 239,
        GenerateSurveuyURL = 240,
        DeleteSamplingQueryDetailsExportData = 241,
        DataArenaDownload = 242,
        CopyProject = 243,
        EditBucketDetails = 244,
        PreviewSurveyRegistration = 245,
        DeleteExpressionData = 246,
        RemoveMember = 247,
        DeleteScheduleBucket = 248,
        ImportPanelistPoints = 249,
        AutoPointApproveProcess = 250,
        BulkClearInterviewData = 251,
        ChangeQuestionOrder = 252,
        MakeQuestionMandatory = 253,
        ManageSurveyThemes = 254

    }
}
