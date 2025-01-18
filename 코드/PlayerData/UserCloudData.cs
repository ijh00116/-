using BlackTree.Definition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree.Model
{
    [Serializable]
    public class UserCloudData
    {
		public UserGoldUpgrade goldUpgrade = new UserGoldUpgrade();
		public UserGoldUpgrade goldtiersecUpgrade = new UserGoldUpgrade();
		public UserStatusUpgrade statusUpgrade = new UserStatusUpgrade();
		public UserStatusUpgrade statusTiersecUpgrade = new UserStatusUpgrade();
		public UserAwakeUpgrade awakeUpgrade = new UserAwakeUpgrade();

		public UserGoodArchive good = new UserGoodArchive();

		public UserFieldArchive field = new UserFieldArchive();

		public UserEquipdatalist weapondata = new UserEquipdatalist();
		public UserEquipdatalist armordata = new UserEquipdatalist();
		public UserEquipdatalist staffdata = new UserEquipdatalist();

		public UserSkillDatas skilldata = new UserSkillDatas();
		public UserEquipedSkill userSkillEquipinfo = new UserEquipedSkill();

		public UserLevelData userLevelData = new UserLevelData();
		public UserAwakeData userAdvancedata = new UserAwakeData();

		public PlayingRecord playingRecord = new PlayingRecord();

		public DungeonData Dungeondata = new DungeonData();

		public OptionValue optiondata = new OptionValue();
		public UserPurchaseArchive inAppPurchase = new UserPurchaseArchive();
		public UserPurchaseArchive inAppSkillPurchase = new UserPurchaseArchive();
		public AttendUserData attendData = new AttendUserData();
		public ClickerDungeonData clickerDungeonData = new ClickerDungeonData();
		public UserResearchInfo researchData = new UserResearchInfo();

		public UserPetData petdata = new UserPetData();
		public UserEquipedPet userPetEquipinfo = new UserEquipedPet();
		public RaidDungeonData userRaidData = new RaidDungeonData();
		public UserStatusData userStatusValue = new UserStatusData();
		public UserOfflineReward offlineReward = new UserOfflineReward();
		public AdsBuffTimeData adsBuffData = new AdsBuffTimeData();
		public TutorialData tutorialData = new TutorialData();

		public ChapterRewardData chapterRewardedData = new ChapterRewardData();
		public UserPurchaseArchiveInAppProduct userInAppProductPurchase = new UserPurchaseArchiveInAppProduct();
		public AdShopProductData adShopProductData = new AdShopProductData();
		public MailRecieveHistory mailRecieveHistory = new MailRecieveHistory();
		public BattlePassHistoryData battlepassHistory = new BattlePassHistoryData();
		public BattlePassPurchaseHistory battlepassPurchaseHistory = new BattlePassPurchaseHistory();
		public UserRuneDatas runeData = new UserRuneDatas();
		public UserEquipedRune runeEquipedInfo = new UserEquipedRune();

		public RaidRankingData userRaidRankingData = new RaidRankingData();
	}

	[Serializable]
	public class UserGoldUpgrade : UserDataBase
	{
		public List<int> upgradeLevels = new List<int>();
	}

	[Serializable]
	public class UserAwakeUpgrade : UserDataBase
	{
		public List<int> upgradeLevels = new List<int>();
		public int AwakeLevel = 0;
	}

	[Serializable]
	public class UserLevelData:UserDataBase
    {
		public int currentLevel=1;
		public double currentExp=0;
    }

	[Serializable]
	public class UserAwakeData:UserDataBase
    {
		public int Grade = 0;
		public List<AdvanceInfo> AdvanceInfo = new List<AdvanceInfo>();
    }

	[Serializable]
	public class AdvanceInfo
    {
		public bool isAdvanced = false;
    }

	[Serializable]
	public class UserStatusUpgrade : UserDataBase
	{
		public List<int> upgradeLevels = new List<int>();
	}

	[Serializable]
	public class UserGoodArchive : UserDataBase
	{
		public List<UserGood> collection = new List<UserGood>();
	}
	[Serializable]
	public class UserGood
	{
		public override int GetHashCode()
		{
			return (int)(value) ^ 142485;
		}
		public double value;
		public double totalValue;
	}
	[Serializable]
	public class UserFieldArchive : UserDataBase
	{
		public int chapter = 0;
		public int stage = 0;
		public int currentKillEnemy = 0;
		public int bestChapter;
		public int bestStage;
		public int bestChapterForBackEndRanking;
		public int bestStageForBackEndRanking;
	}

	[Serializable]
	public class UserEquipdatalist: UserDataBase
	{
		public int currentEquipIndex=0;
		public List<UserEquipdata> itemdatas =new List<UserEquipdata>();
		public int currentSummonExp=0;
		public int summonLevel=1;
		public int adsummonCount = 0;
	}
	[Serializable]
	public class UserEquipdata : UserDataBase
	{
		public int Obtaincount = 0;//획득한 갯수
		public int Obtainlv = 0;//획득한 템의 레벨
		public int AwakeLv = 0;
	}

	[Serializable]
	public class UserSkillDatas: UserDataBase
	{
		public List<UserSkillInfo> collection = new List<UserSkillInfo>();
		public List<bool> skillUnlockState = new List<bool>();
		public int summonLevel = 1;
		public int currentSummonExp = 0;
		public bool isAutoSkill=false;
		public int adsummonCount = 0;
	}

	[Serializable]
	public class UserSkillInfo
	{
		public bool Unlock=false;
		public int level=1;//level 기본값은 1
		public int Obtaincount=0;
		public float elapsedCooltime = -1.0f;
		public float elapsedCooltimeinContent = -1.0f;
		public int AwakeLv = 0;//초월 기본값은 0
	}

	[Serializable]
	public class UserEquipedSkill:UserDataBase
    {
		public int currentEquipSkillIndex = 0;
		public List<EquipedSkillList> deckList;
		public UserEquipedSkill()
        {
			deckList = new List<EquipedSkillList>();
        }
    }

	[Serializable]
	public class EquipedSkillList
    {
		public List<SkillKey> skills = new List<SkillKey>();
    }
	[Serializable]
	public class RelicList:UserDataBase
	{
		public List<UserRelicinfo> collections = new List<UserRelicinfo>();
	}

	[Serializable]
	public class UserRelicinfo
    {
		public int amount;
		public int lv;
    }

	[Serializable]
	public class PlayingRecord : UserDataBase
	{
		public PlayingQuestRecord mainQuest = new PlayingQuestRecord();
		public List<PlayingQuestRecord> dailyQuestCollections = new List<PlayingQuestRecord>();
		public List<PlayingQuestRecord> repeatQuestCollections = new List<PlayingQuestRecord>();

		public int savedDay = -1;
		public int dailyQuestCompleteCount=0;
		public bool dailyCompleteQuestRewardRecieve = false;

		public int PlayingMainQuestCount = 0;

		public bool isFreeSkillAwakeComplete = false;
		public void InitDay()
        {
			dailyCompleteQuestRewardRecieve = false;
			for (int i = 0; i < dailyQuestCollections.Count; i++)
			{
				dailyQuestCollections[i].InitDay(true);
			}
			dailyQuestCompleteCount = 0;
		}
	}

	[Serializable]
	public class PlayingQuestRecord
	{
		public int id=-1;
		public QuestType questType;
		public int playingCount;
		public bool isRewarded=false;
		public bool isAdRewarded = false;
		public PlayingQuestRecord()
        {
			questType = QuestType.KillEnemy;
			playingCount = 0;
			isRewarded = false;
			isAdRewarded = false;
		}
		public void Init(DataQuest questInfo)
        {
			id = questInfo.id;
			questType = questInfo.questType;
			playingCount = 0;
			isRewarded = false;
			isAdRewarded = false;
		}
		public void CountUp(int _count)
        {
			playingCount += _count;
		}
		public void InitDay(bool otherDay)
        {
			if(otherDay)
            {
				playingCount = 0;
				isRewarded = false;
				isAdRewarded = false;
			}
        }
    }

	[Serializable]
	public class DungeonData : UserDataBase
	{
		public List<DungeonUserInfo> dungeoninfo = new List<DungeonUserInfo>();
		public List<int> runeDungeonRewardHistory = new List<int>();
    }

	[Serializable]
	public class RaidDungeonData:UserDataBase
    {
		public double bestDamage=0;
		public int todayParticipateCount=0;
		public string currentRegisteredRankUUID=null;
		public string currentRegisteredRankTitle = null;
		public bool isRaidUnlocked=false;
    }

	[Serializable]
	public class DungeonUserInfo:UserDataBase
    {
		public int bestLevel;
		public int bestKillCount;
		public double bestDamage;
	}

	[Serializable]
	public class OptionValue:UserDataBase
    {
		public SocialLoginType recentSocialType = SocialLoginType.None;
		public string useruuid = null;
		public string nickname = null;
		public string lastSaveTime;
		public int lastLoginedDay = -1;
		public string backenduuid = null;

		public float bgmSound= 1.0f;
		public float effectSound = 1.0f;
		public bool appearDmg = true;
		public bool appearEffect = true;
		public bool autoSaveMode = true;
		public bool autoChallengeBoss = true;
		public bool camShake = true;
		public PushAlaramType pushAlarm = PushAlaramType.NotDecided;
		public string nicknameChangeUnlocktime = null;

		public bool isGuest = false;
		public bool isReview = false;
		public string informVersion = null;

		public bool isGoogleLogin = true;
	}

	[Serializable]
	public class UserStatusData:UserDataBase
    {
		public double hp=0;
		public double shield=0;
    }

	[Serializable]
	public class UserPurchaseArchive : UserDataBase
	{
		public List<UserInAppPurchase> collection = new List<UserInAppPurchase>();

		public int adDiapurchaseCount;
		public int adSkillAwakepurchaseCount;
		public int purchaseVip=0;
		public bool purchaseAds=false;
		public bool isVipDailyRewardGet = false;

		public int newPackageProductKey = -1;
	}

	[Serializable]
	public class UserInAppPurchase:UserDataBase
	{
		public int currBoughtCount = 0;
		public string lastTimePackage=null;
		public string expiredDay=null;
		public bool isPurchased = false;
		public bool isSkillWindowPopuped = false;
	}

	[Serializable]
	public class UserPurchaseArchiveInAppProduct:UserDataBase
    {
		public List<UserInAppPurchase> collection = new List<UserInAppPurchase>();
	}

	[Serializable]
	public class AttendUserData : UserDataBase
	{
		public int lastRewardedDay = -1;
		public int lastRewardIndex = -1;

	}

	[Serializable]
	public class ClickerDungeonData : UserDataBase
	{
		public int todayEarnRewardCount;
		public int currentKillCount;
		public int clickerSubUnitHp = 50;
		public int clickerGageCount = 0;
		public List<RewardTypes> todayEarnRewardtypeList=new List<RewardTypes>();
		public List<double> todayEarnRewardAmountList=new List<double>();
	}
	[Serializable]
	public class UserResearchInfo : UserDataBase
	{
		public List<UserResearchUpgradeData> data = new List<UserResearchUpgradeData>();
		public List<ResearchUpgradeKey> currentResearchKeylist=new List<ResearchUpgradeKey>();
	}

	[Serializable]
	public class UserResearchUpgradeData:UserDataBase
    {
		public int level;
		public ResearchUpgradeStateKey upgradeState =ResearchUpgradeStateKey.Ready;
		public string scheduledExittime;

		public void SetUpgradeComplete()
        {
			scheduledExittime = null;
			upgradeState = ResearchUpgradeStateKey.Ready;
        }
		
		public void SetUpgradeStart(DateTime exitTime)
        {
			scheduledExittime = exitTime.ToIsoString();
			upgradeState = ResearchUpgradeStateKey.Progressing;
		}
    }


	[Serializable]
	public class UserPetData : UserDataBase
	{
		public List<UserPetInfo> collection = new List<UserPetInfo>();
		public List<bool> petUnlockState = new List<bool>();
		public int summonLevel = 1;
		public int currentSummonExp = 0;
		public int adsummonCount = 0;
	}

	[Serializable]
	public class UserPetInfo
	{
		public int level = 0;
		public int Obtaincount = 0;
		public int unlockSkillLevel = 0;
		public float elapsedCooltime = -1.0f;
	}

	[Serializable]
	public class UserEquipedPet : UserDataBase
	{
		public int currentEquipPetDeckIndex = 0;
		public List<EquipedPetList> deckList;
		public UserEquipedPet()
		{
			deckList = new List<EquipedPetList>();
		}
	}

	[Serializable]
	public class EquipedPetList
	{
		public List<int> petIDlist= new List<int>();
	}

	[Serializable]
	public class UserOfflineReward : UserDataBase
	{
		public string lastReceiveRewardTime = "";
	}

	[Serializable]
	public class AdsBuffTimeData:UserDataBase
    {
		public List<bool> adsFreeBuffComplete = new List<bool>();
		public List<int> adsValudata = new List<int>();
		public List<float> adsLeftTimeData = new List<float>();
    }

	[Serializable]
	public class TutorialData: UserDataBase
	{
		public List<TutorialType> tutorialType= new List<TutorialType>();
		public List<bool> tutorialCleared = new List<bool>();

		public bool isFirstSummonItems = true;
		public bool isFirstSummonSkills = true;
	}

	[Serializable]
	public class ChapterRewardData : UserDataBase
	{
		//public List<bool> rewardedList=new List<bool>();
		public int LastRewardIndex=-1;
	}

	[Serializable]
	public class DiaPurchasedHistory: UserDataBase
	{
		public List<bool> rewardedList = new List<bool>();
	}

	[Serializable]
	public class AdShopProductData : UserDataBase
	{
		public List<AdShopProductHistory> boughtProductList = new List<AdShopProductHistory>();
	}

	[Serializable]
	public class AdShopProductHistory : UserDataBase
    {
		public int boughtCount;
    }

	[Serializable]
	public class MailRecieveHistory:UserDataBase
    {
		public List<bool> recieved=new List<bool>();
    }

	[Serializable]
	public class BattlePassHistoryData: UserDataBase
	{
		public BattlsPassHistory levelPassHistory_0 = new BattlsPassHistory();
		public BattlsPassHistory levelPassHistory_1 = new BattlsPassHistory();
		public BattlsPassHistory levelPassHistory_2 = new BattlsPassHistory();
		public BattlsPassHistory chapterPassHistory_0 = new BattlsPassHistory();
		public BattlsPassHistory chapterPassHistory_1 = new BattlsPassHistory();
		public BattlsPassHistory chapterPassHistory_2 = new BattlsPassHistory();
	}
	
	[Serializable]
	public class BattlsPassHistory : UserDataBase
	{
		public List<bool> normalRecieved = new List<bool>();
		public List<bool> premiumRecieved = new List<bool>();
	}

	[Serializable]
	public class BattlePassPurchaseHistory : UserDataBase
	{
		public List<bool> levelPassPurchased = new List<bool>();
		public List<bool> chapterPassPurchased = new List<bool>();
	}

	[Serializable]
	public class UserRuneDatas : UserDataBase
	{
		public List<UserRuneInfo> collection = new List<UserRuneInfo>();
		public int summonLevel = 1;
		public int currentSummonExp = 0;
		public int adsummonCount = 0;
	}

	[Serializable]
	public class UserRuneInfo
	{
		public bool Unlock = false;
		public int Obtainlv = 0;//level 기본값은 1
		public int Obtaincount = 0;
		public int AwakeLv = 0;//초월 기본값은 0
	}

	[Serializable]
	public class UserEquipedRune : UserDataBase
	{
		public int currentEquipRuneIndex = 0;
		public List<EquipedRuneList> deckList;
		public UserEquipedRune()
		{
			deckList = new List<EquipedRuneList>();
		}
	}

	[Serializable]
	public class EquipedRuneList
	{
		public List<int> runes= new List<int>();
	}

	[Serializable]
	public class RaidRankingData: UserDataBase
	{
		public bool isRaidFirstSet=false;
    }
	public abstract class UserDataBase
	{
		private int _prevHash;
		public bool IsDirty { get; private set; }

		public UserDataBase UpdateHash()
		{
			var data = GetDataString();
			_prevHash = data.GetHashCode();
			return this;
		}
		public bool IsValidHash()
		{
			var data = GetDataString();
			return data.GetHashCode() == _prevHash;
		}
		public UserDataBase SetDirty(bool flag)
		{
			IsDirty = flag;
			return this;
		}
		public string GetDataString()
		{
			var res = JsonUtility.ToJson(this);
			return res;
		}
	}
}
