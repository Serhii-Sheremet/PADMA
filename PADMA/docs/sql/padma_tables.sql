BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "APPSETTING" (
	"ID"	INTEGER,
	"GROUPCODE"	TEXT,
	"SETTINGCODE"	TEXT,
	"ACTIVE"	SMALLINT,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "APP_META" (
	"KEY"	TEXT,
	"VALUE"	TEXT,
	PRIMARY KEY("KEY")
);
CREATE TABLE IF NOT EXISTS "APP_TEXTS" (
	"ID"	INTEGER,
	"NATIVETEXT"	TEXT,
	"FOREIGNTEXT"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "COLOR" (
	"ID"	INTEGER,
	"CODE"	TEXT,
	"ARGBVALUE"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "COLOR_DESC" (
	"ID"	INTEGER,
	"COLORID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID")
);
CREATE TABLE IF NOT EXISTS "DVLINENAMES" (
	"ID"	INTEGER,
	"CODE"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "DVLINENAMES_DESC" (
	"ID"	INTEGER,
	"DVLINENAMESID"	INTEGER,
	"SHORTNAME"	TEXT,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("DVLINENAMESID") REFERENCES "DVLINENAMES"("ID")
);
CREATE TABLE IF NOT EXISTS "ECLIPSE" (
	"ID"	INTEGER,
	"ECLIPSECODE"	VARCHAR(10),
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "ECLIPSE_DESC" (
	"ID"	INTEGER,
	"ECLIPSEID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("ECLIPSEID") REFERENCES "ECLIPSE"("ID")
);
CREATE TABLE IF NOT EXISTS "FONTLIST" (
	"ID"	INTEGER,
	"FONTID"	INTEGER,
	"CODE"	TEXT,
	"FONTSTYLEID"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("FONTID") REFERENCES "SYSTEMFONT"("ID")
);
CREATE TABLE IF NOT EXISTS "FONTLIST_DESC" (
	"ID"	INTEGER,
	"FONTLISTID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("FONTLISTID") REFERENCES "FONTLIST"("ID")
);
CREATE TABLE IF NOT EXISTS "GHATI60" (
	"ID"	INTEGER,
	"POSITION"	SMALLINT,
	"COLORID"	INTEGER,
	"GHATI60CODE"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID")
);
CREATE TABLE IF NOT EXISTS "GHATI60_DESC" (
	"ID"	INTEGER,
	"GHATI60ID"	INTEGER,
	"SHORTNAME"	TEXT,
	"NAME"	TEXT,
	"DESCRIPTION"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("GHATI60ID") REFERENCES "GHATI60_OLD"("ID")
);
CREATE TABLE IF NOT EXISTS "KARANA" (
	"ID"	INTEGER,
	"TITHIID"	INTEGER,
	"POSITION"	SMALLINT,
	"COLORID"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID"),
	FOREIGN KEY("TITHIID") REFERENCES "TITHI"("ID")
);
CREATE TABLE IF NOT EXISTS "KARANA_DESC" (
	"ID"	INTEGER,
	"KARANAID"	INTEGER,
	"NAME"	TEXT,
	"UPRAVITEL"	TEXT,
	"GOODFOR"	TEXT,
	"BADFOR"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("KARANAID") REFERENCES "KARANA"("ID")
);
CREATE TABLE IF NOT EXISTS "LANGUAGE" (
	"ID"	INTEGER,
	"LANGUAGECODE"	VARCHAR(2),
	"CULTURECODE"	VARCHAR(5),
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "LANGUAGE_DESC" (
	"ID"	INTEGER,
	"LANGUAGEID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("LANGUAGEID") REFERENCES "LANGUAGE"("ID")
);
CREATE TABLE IF NOT EXISTS "LOCATION" (
	"ID"	INTEGER,
	"LOCALITY"	TEXT,
	"LATITUDE"	TEXT,
	"LONGITUDE"	TEXT,
	"REGION"	TEXT,
	"STATE"	TEXT,
	"COUNTRY"	TEXT,
	"COUNTRYCODE"	VARCHAR(2),
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "MASA" (
	"ID"	INTEGER,
	"ZODIAKID"	INTEGER,
	"SHUNYANAKSHATRA"	TEXT,
	"SHUNYATITHI"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("ZODIAKID") REFERENCES "ZODIAK_OLD"("ID")
);
CREATE TABLE IF NOT EXISTS "MASA_DESC" (
	"ID"	INTEGER,
	"MASAID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("MASAID") REFERENCES "MASA"("ID")
);
CREATE TABLE IF NOT EXISTS "MRITYUBHAGA" (
	"ID"	INTEGER,
	"PLANETID"	INTEGER,
	"ZODIACID"	INTEGER,
	"DEGREE"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("PLANETID") REFERENCES "PLANET"("ID"),
	FOREIGN KEY("ZODIACID") REFERENCES "ZODIAC"("ID")
);
CREATE TABLE IF NOT EXISTS "MUHURTA" (
	"ID"	INTEGER,
	"COLORID"	INTEGER,
	"MUHURTACODE"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID")
);
CREATE TABLE IF NOT EXISTS "MUHURTA30" (
	"ID"	INTEGER,
	"COLORID"	INTEGER,
	"MUHURTA30CODE"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID")
);
CREATE TABLE IF NOT EXISTS "MUHURTA30_DESC" (
	"ID"	INTEGER,
	"MUHURTA30ID"	INTEGER,
	"SHORTNAME"	TEXT,
	"NAME"	TEXT,
	"DESCRIPTION"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("MUHURTA30ID") REFERENCES "MUHURTA30_OLD"("ID")
);
CREATE TABLE IF NOT EXISTS "MUHURTA_DESC" (
	"ID"	INTEGER,
	"MUHURTAID"	INTEGER,
	"NAME"	TEXT,
	"SHORTNAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("MUHURTAID") REFERENCES "MUHURTA"("ID")
);
CREATE TABLE IF NOT EXISTS "NAKSHATRA" (
	"ID"	INTEGER,
	"NAKSHATRACODE"	TEXT,
	"COLORID"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID")
);
CREATE TABLE IF NOT EXISTS "NAKSHATRA_DESC" (
	"ID"	INTEGER,
	"NAKSHATRAID"	INTEGER,
	"NAME"	TEXT,
	"SHORTNAME"	TEXT,
	"RULER"	TEXT,
	"NATURE"	TEXT,
	"DESCRIPTION"	TEXT,
	"GOODFOR"	TEXT,
	"BADFOR"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("NAKSHATRAID") REFERENCES "NAKSHATRA"("ID")
);
CREATE TABLE IF NOT EXISTS "NITYAYOGA" (
	"ID"	INTEGER,
	"NYCODE"	TEXT,
	"COLORID"	INTEGER,
	"NAKSHATRAID"	INTEGER,
	"YOGIPLANETID"	INTEGER,
	"AVAYOGIPLANETID"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("AVAYOGIPLANETID") REFERENCES "PLANET"("ID"),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID"),
	FOREIGN KEY("NAKSHATRAID") REFERENCES "NAKSHATRA"("ID"),
	FOREIGN KEY("YOGIPLANETID") REFERENCES "PLANET"("ID")
);
CREATE TABLE IF NOT EXISTS "NITYAYOGA_DESC" (
	"ID"	INTEGER,
	"NITYAYOGAID"	INTEGER,
	"NAME"	TEXT,
	"DEITY"	TEXT,
	"MEANING"	TEXT,
	"DESCRIPTION"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("NITYAYOGAID") REFERENCES "NITYAYOGA"("ID")
);
CREATE TABLE IF NOT EXISTS "PADA" (
	"ID"	INTEGER,
	"ZODIACID"	INTEGER,
	"NAKSHATRAID"	INTEGER,
	"PADANUMBER"	INTEGER,
	"DREKKANA"	INTEGER,
	"SPECIALNAVAMSA"	TEXT,
	"NAVAMSA"	INTEGER,
	"COLORID"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID"),
	FOREIGN KEY("NAKSHATRAID") REFERENCES "NAKSHATRA"("ID"),
	FOREIGN KEY("ZODIACID") REFERENCES "ZODIAC"("ID")
);
CREATE TABLE IF NOT EXISTS "PLANET" (
	"ID"	INTEGER,
	"PLANETCODE"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "PLANET_DESC" (
	"ID"	INTEGER,
	"PLANETID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("PLANETID") REFERENCES "PLANET"("ID")
);
CREATE TABLE IF NOT EXISTS "PROFILE" (
	"ID"	INTEGER,
	"PROFILENAME"	TEXT,
	"PERSONNAME"	TEXT,
	"PERSONSURNAME"	TEXT,
	"DATEOFBIRTH"	TEXT,
	"PLACEOFBIRTHID"	INTEGER,
	"PLACEOFLIVINGID"	INTEGER,
	"MESSAGE"	TEXT,
	"CHECKED"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("PLACEOFBIRTHID") REFERENCES "LOCATION"("ID"),
	FOREIGN KEY("PLACEOFLIVINGID") REFERENCES "LOCATION"("ID")
);
CREATE TABLE IF NOT EXISTS "SPECIALNAVAMSHA_DESC" (
	"ID"	INTEGER,
	"SPECIALNAVAMSHAID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "SYSTEMFONT" (
	"ID"	INTEGER,
	"APPMAIN"	SMALLINT,
	"SYSTEMNAME"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "TARABALA" (
	"ID"	INTEGER,
	"COLORID"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID")
);
CREATE TABLE IF NOT EXISTS "TARABALA_DESC" (
	"ID"	INTEGER,
	"TARABALAID"	INTEGER,
	"NAME"	TEXT,
	"SHORTNAME"	TEXT,
	"DESCRIPTION"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("TARABALAID") REFERENCES "TARABALA"("ID")
);
CREATE TABLE IF NOT EXISTS "TITHI" (
	"ID"	INTEGER,
	"COLORID"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID")
);
CREATE TABLE IF NOT EXISTS "TITHI_DESC" (
	"ID"	INTEGER,
	"TITHIID"	INTEGER,
	"NAME"	TEXT,
	"SHORTNAME"	TEXT,
	"UPRAVITEL"	TEXT,
	"TYPE"	TEXT,
	"GOODFOR"	TEXT,
	"BADFOR"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("TITHIID") REFERENCES "TITHI"("ID")
);
CREATE TABLE IF NOT EXISTS "TRANSIT" (
	"ID"	INTEGER,
	"PLANETID"	INTEGER,
	"HOUSE"	INTEGER,
	"COLORID"	INTEGER,
	"VEDHA"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID"),
	FOREIGN KEY("PLANETID") REFERENCES "PLANET"("ID")
);
CREATE TABLE IF NOT EXISTS "TRANSIT_DESC" (
	"ID"	INTEGER,
	"TRANSITID"	INTEGER,
	"DESCRIPTION"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("TRANSITID") REFERENCES "TRANSIT"("ID")
);
CREATE TABLE IF NOT EXISTS "TRANSIT_EVENTS" (
	"ID"	INTEGER,
	"PROFILEID"	INTEGER,
	"EVENTDATE"	VARCHAR(20),
	"LOCATIONID"	INTEGER,
	"EVENTNAME"	TEXT,
	"DESCRIPTION"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("LOCATIONID") REFERENCES "LOCATION"("ID"),
	FOREIGN KEY("PROFILEID") REFERENCES "PROFILE"("ID")
);
CREATE TABLE IF NOT EXISTS "USER_EVENTS" (
	"ID"	INTEGER,
	"PROFILEID"	INTEGER,
	"DATESTART"	VARCHAR(20),
	"DATEEND"	VARCHAR(20),
	"NAME"	TEXT,
	"MESSAGE"	TEXT,
	"ARGBVALUE"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("PROFILEID") REFERENCES "PROFILE"("ID")
);
CREATE TABLE IF NOT EXISTS "YOGA" (
	"ID"	INTEGER,
	"COLORID"	INTEGER,
	"YOGACODE"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID")
);
CREATE TABLE IF NOT EXISTS "YOGA_DESC" (
	"ID"	INTEGER,
	"YOGAID"	INTEGER,
	"NAME"	TEXT,
	"SHORTNAME"	VARCHAR(3),
	"DESCRIPTION"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("YOGAID") REFERENCES "YOGA"("ID")
);
CREATE TABLE IF NOT EXISTS "ZODIAC" (
	"ID"	INTEGER,
	"ZODIACCODE"	VARCHAR(3),
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "ZODIAC_DESC" (
	"ID"	INTEGER,
	"ZODIACID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("ZODIACID") REFERENCES "ZODIAC"("ID")
);
INSERT INTO "APPSETTING" VALUES (1,'LANGUAGE','ENGLISH',0);
INSERT INTO "APPSETTING" VALUES (2,'LANGUAGE','UKRAINIAN',0);
INSERT INTO "APPSETTING" VALUES (3,'LANGUAGE','POLISH',0);
INSERT INTO "APPSETTING" VALUES (4,'LANGUAGE','RUSSIAN',1);
INSERT INTO "APPSETTING" VALUES (5,'TRANSIT','MOON',0);
INSERT INTO "APPSETTING" VALUES (6,'TRANSIT','LAGNA',0);
INSERT INTO "APPSETTING" VALUES (7,'TRANSIT','MOONANDLAGNA',1);
INSERT INTO "APPSETTING" VALUES (8,'HORA','HORADAYNIGHT',0);
INSERT INTO "APPSETTING" VALUES (9,'HORA','HORAEQUAL',1);
INSERT INTO "APPSETTING" VALUES (10,'HORA','HORAFROM6',0);
INSERT INTO "APPSETTING" VALUES (11,'MUHURTAGHATI','MUHURTAGHATIDAYNIGHT',0);
INSERT INTO "APPSETTING" VALUES (12,'MUHURTAGHATI','MUHURTAGHATIEQUAL',1);
INSERT INTO "APPSETTING" VALUES (13,'MUHURTAGHATI','MUHURTAGHATIFROM6',0);
INSERT INTO "APPSETTING" VALUES (14,'MRITYUBHAGA','NEQUAL',0);
INSERT INTO "APPSETTING" VALUES (15,'MRITYUBHAGA','NLESS',0);
INSERT INTO "APPSETTING" VALUES (16,'MRITYUBHAGA','NMORE',0);
INSERT INTO "APPSETTING" VALUES (17,'MRITYUBHAGA','NERNST',1);
INSERT INTO "APPSETTING" VALUES (18,'NODE','MEAN',1);
INSERT INTO "APPSETTING" VALUES (19,'NODE','TRUE',0);
INSERT INTO "APPSETTING" VALUES (20,'WEEK','WEEKSUNDAY',0);
INSERT INTO "APPSETTING" VALUES (21,'WEEK','WEEKMONDAY',1);
INSERT INTO "APPSETTING" VALUES (22,'SUNRISE','TIP',1);
INSERT INTO "APPSETTING" VALUES (23,'SUNRISE','CENTER',0);
INSERT INTO "APP_META" VALUES ('DB_VERSION','0.0.19');
INSERT INTO "APP_TEXTS" VALUES (1,'Language','Language','en');
INSERT INTO "APP_TEXTS" VALUES (2,'Language','Мова','uk');
INSERT INTO "APP_TEXTS" VALUES (3,'Language','Język','pl');
INSERT INTO "APP_TEXTS" VALUES (4,'Language','Язык','ru');
INSERT INTO "APP_TEXTS" VALUES (5,'English','English','en');
INSERT INTO "APP_TEXTS" VALUES (6,'English','Англійська','uk');
INSERT INTO "APP_TEXTS" VALUES (7,'English','Angielski','pl');
INSERT INTO "APP_TEXTS" VALUES (8,'English','Английский','ru');
INSERT INTO "APP_TEXTS" VALUES (9,'Ukrainian','Ukrainian','en');
INSERT INTO "APP_TEXTS" VALUES (10,'Ukrainian','Українська','uk');
INSERT INTO "APP_TEXTS" VALUES (11,'Ukrainian','Ukraiński','pl');
INSERT INTO "APP_TEXTS" VALUES (12,'Ukrainian','Украинский','ru');
INSERT INTO "APP_TEXTS" VALUES (13,'Polish','Polish','en');
INSERT INTO "APP_TEXTS" VALUES (14,'Polish','Польська','uk');
INSERT INTO "APP_TEXTS" VALUES (15,'Polish','Polski','pl');
INSERT INTO "APP_TEXTS" VALUES (16,'Polish','Польский','ru');
INSERT INTO "APP_TEXTS" VALUES (17,'Russian','Russian','en');
INSERT INTO "APP_TEXTS" VALUES (18,'Russian','Російська','uk');
INSERT INTO "APP_TEXTS" VALUES (19,'Russian','Rosyjski','pl');
INSERT INTO "APP_TEXTS" VALUES (20,'Russian','Русский','ru');
INSERT INTO "APP_TEXTS" VALUES (21,'Save changes?','Save changes?','en');
INSERT INTO "APP_TEXTS" VALUES (22,'Save changes?','Зберегти зміни?','uk');
INSERT INTO "APP_TEXTS" VALUES (23,'Save changes?','Zapisać zmiany?','pl');
INSERT INTO "APP_TEXTS" VALUES (24,'Save changes?','Сохранить изменения?','ru');
INSERT INTO "APP_TEXTS" VALUES (25,'Apply new language setting?','Apply new language setting?','en');
INSERT INTO "APP_TEXTS" VALUES (26,'Apply new language setting?','Застосувати нові налаштування мови?','uk');
INSERT INTO "APP_TEXTS" VALUES (27,'Apply new language setting?','Zastosować nowe ustawienia języka?','pl');
INSERT INTO "APP_TEXTS" VALUES (28,'Apply new language setting?','Применить новые настройки языка?','ru');
INSERT INTO "APP_TEXTS" VALUES (29,'Yes','Yes','en');
INSERT INTO "APP_TEXTS" VALUES (30,'Yes','Так','uk');
INSERT INTO "APP_TEXTS" VALUES (31,'Yes','Tak','pl');
INSERT INTO "APP_TEXTS" VALUES (32,'Yes','Да','ru');
INSERT INTO "APP_TEXTS" VALUES (33,'No','No','en');
INSERT INTO "APP_TEXTS" VALUES (34,'No','Ні','uk');
INSERT INTO "APP_TEXTS" VALUES (35,'No','Nie','pl');
INSERT INTO "APP_TEXTS" VALUES (36,'No','Нет','ru');
INSERT INTO "APP_TEXTS" VALUES (37,'First day of week','First day of week','en');
INSERT INTO "APP_TEXTS" VALUES (38,'First day of week','Перший день тижня','uk');
INSERT INTO "APP_TEXTS" VALUES (39,'First day of week','Pierwszy dzień tygodnia','pl');
INSERT INTO "APP_TEXTS" VALUES (40,'First day of week','Первый день недели','ru');
INSERT INTO "APP_TEXTS" VALUES (41,'Monday','Monday','en');
INSERT INTO "APP_TEXTS" VALUES (42,'Monday','Понеділок','uk');
INSERT INTO "APP_TEXTS" VALUES (43,'Monday','Poniedziałek','pl');
INSERT INTO "APP_TEXTS" VALUES (44,'Monday','Понедельник','ru');
INSERT INTO "APP_TEXTS" VALUES (45,'Sunday','Sunday','en');
INSERT INTO "APP_TEXTS" VALUES (46,'Sunday','Неділя','uk');
INSERT INTO "APP_TEXTS" VALUES (47,'Sunday','Niedziela','pl');
INSERT INTO "APP_TEXTS" VALUES (48,'Sunday','Воскресенье','ru');
INSERT INTO "APP_TEXTS" VALUES (49,'Apply new setting for first day of week?','Apply new setting for first day of week?','en');
INSERT INTO "APP_TEXTS" VALUES (50,'Apply new setting for first day of week?','Застосувати нові налаштування першого дня тижня?','uk');
INSERT INTO "APP_TEXTS" VALUES (51,'Apply new setting for first day of week?','Zastosować nowe ustawienia pierwszego dnia tygodnia?','pl');
INSERT INTO "APP_TEXTS" VALUES (52,'Apply new setting for first day of week?','Применить новые настройки первого дня недели?','ru');
INSERT INTO "APP_TEXTS" VALUES (53,'Choose application language:','Choose application language:','en');
INSERT INTO "APP_TEXTS" VALUES (54,'Choose application language:','Оберіть мову застосунку:','uk');
INSERT INTO "APP_TEXTS" VALUES (55,'Choose application language:','Wybierz język aplikacji:','pl');
INSERT INTO "APP_TEXTS" VALUES (56,'Choose application language:','Выберите язык приложения:','ru');
INSERT INTO "APP_TEXTS" VALUES (57,'Choose the first day of the week:','Choose the first day of the week:','en');
INSERT INTO "APP_TEXTS" VALUES (58,'Choose the first day of the week:','Оберіть перший день тижня:','uk');
INSERT INTO "APP_TEXTS" VALUES (59,'Choose the first day of the week:','Wybierz pierwszy dzień tygodnia:','pl');
INSERT INTO "APP_TEXTS" VALUES (60,'Choose the first day of the week:','Выберите первый день недели:','ru');
INSERT INTO "APP_TEXTS" VALUES (61,'Configuration Updated','Configuration Updated','en');
INSERT INTO "APP_TEXTS" VALUES (62,'Configuration Updated','Конфігурацію оновлено','uk');
INSERT INTO "APP_TEXTS" VALUES (63,'Configuration Updated','Konfiguracja zaktualizowana','pl');
INSERT INTO "APP_TEXTS" VALUES (64,'Configuration Updated','Конфигурация обновлена','ru');
INSERT INTO "APP_TEXTS" VALUES (65,'Settings have been successfully applied.','Settings have been successfully applied.','en');
INSERT INTO "APP_TEXTS" VALUES (66,'Settings have been successfully applied.','Налаштування успішно застосовано.','uk');
INSERT INTO "APP_TEXTS" VALUES (67,'Settings have been successfully applied.','Ustawienia zostały pomyślnie zastosowane.','pl');
INSERT INTO "APP_TEXTS" VALUES (68,'Settings have been successfully applied.','Настройки успешно применены.','ru');
INSERT INTO "APP_TEXTS" VALUES (69,'Planetary transits','Planetary transits','en');
INSERT INTO "APP_TEXTS" VALUES (70,'Planetary transits','Транзити планет','uk');
INSERT INTO "APP_TEXTS" VALUES (71,'Planetary transits','Tranzyty planet','pl');
INSERT INTO "APP_TEXTS" VALUES (72,'Planetary transits','Транзиты планет','ru');
INSERT INTO "APP_TEXTS" VALUES (73,'Nodes (Rahu and Ketu)','Nodes (Rahu and Ketu)','en');
INSERT INTO "APP_TEXTS" VALUES (74,'Nodes (Rahu and Ketu)','Вузли (Раху і Кету)','uk');
INSERT INTO "APP_TEXTS" VALUES (75,'Nodes (Rahu and Ketu)','Węzły (Rahu i Ketu)','pl');
INSERT INTO "APP_TEXTS" VALUES (76,'Nodes (Rahu and Ketu)','Узлы (Раху и Кету)','ru');
INSERT INTO "APP_TEXTS" VALUES (77,'Hora','Hora','en');
INSERT INTO "APP_TEXTS" VALUES (78,'Hora','Гора','uk');
INSERT INTO "APP_TEXTS" VALUES (79,'Hora','Hora','pl');
INSERT INTO "APP_TEXTS" VALUES (80,'Hora','Хора','ru');
INSERT INTO "APP_TEXTS" VALUES (81,'30 Muhurtas (60 Ghatis)','30 Muhurtas (60 Ghatis)','en');
INSERT INTO "APP_TEXTS" VALUES (82,'30 Muhurtas (60 Ghatis)','30 Мухурт (60 Гхаті)','uk');
INSERT INTO "APP_TEXTS" VALUES (83,'30 Muhurtas (60 Ghatis)','30 Muhurt (60 Ghati)','pl');
INSERT INTO "APP_TEXTS" VALUES (84,'30 Muhurtas (60 Ghatis)','30 Мухурт (60 Гхати)','ru');
INSERT INTO "APP_TEXTS" VALUES (85,'Mrityu Bhaga','Mrityu Bhaga','en');
INSERT INTO "APP_TEXTS" VALUES (86,'Mrityu Bhaga','Мрітью Бхага','uk');
INSERT INTO "APP_TEXTS" VALUES (87,'Mrityu Bhaga','Mrityu Bhaga','pl');
INSERT INTO "APP_TEXTS" VALUES (88,'Mrityu Bhaga','Мритью Бхага','ru');
INSERT INTO "APP_TEXTS" VALUES (89,'Sunrise calculation','Sunrise calculation','en');
INSERT INTO "APP_TEXTS" VALUES (90,'Sunrise calculation','Розрахунок сходу Сонця','uk');
INSERT INTO "APP_TEXTS" VALUES (91,'Sunrise calculation','Obliczanie wschodu słońca','pl');
INSERT INTO "APP_TEXTS" VALUES (92,'Sunrise calculation','Расчет восхода Солнца','ru');
INSERT INTO "APP_TEXTS" VALUES (93,'Settings','Settings','en');
INSERT INTO "APP_TEXTS" VALUES (94,'Settings','Налаштування','uk');
INSERT INTO "APP_TEXTS" VALUES (95,'Settings','Ustawienia','pl');
INSERT INTO "APP_TEXTS" VALUES (96,'Settings','Настройки','ru');
INSERT INTO "APP_TEXTS" VALUES (97,'Choose how to display planetary transits:','Choose how to display planetary transits:','en');
INSERT INTO "APP_TEXTS" VALUES (98,'Choose how to display planetary transits:','Виберіть варіант відображення транзитів планет:','uk');
INSERT INTO "APP_TEXTS" VALUES (99,'Choose how to display planetary transits:','Wybierz sposób wyświetlania tranzytów planetarnych:','pl');
INSERT INTO "APP_TEXTS" VALUES (100,'Choose how to display planetary transits:','Выберите вариант отображения транзитов планет:','ru');
INSERT INTO "APP_TEXTS" VALUES (101,'From natal Moon','From natal Moon','en');
INSERT INTO "APP_TEXTS" VALUES (102,'From natal Moon','Від натального Місяця','uk');
INSERT INTO "APP_TEXTS" VALUES (103,'From natal Moon','Od Księżyca natalnego','pl');
INSERT INTO "APP_TEXTS" VALUES (104,'From natal Moon','От натальной Луны','ru');
INSERT INTO "APP_TEXTS" VALUES (105,'From Ascendant (Lagna)','From Ascendant (Lagna)','en');
INSERT INTO "APP_TEXTS" VALUES (106,'From Ascendant (Lagna)','Від Асцендента (Лагни)','uk');
INSERT INTO "APP_TEXTS" VALUES (107,'From Ascendant (Lagna)','Od Ascendentu (Lagna)','pl');
INSERT INTO "APP_TEXTS" VALUES (108,'From Ascendant (Lagna)','От Асцендента (Лагны)','ru');
INSERT INTO "APP_TEXTS" VALUES (109,'From both natal Moon and Ascendant','From both natal Moon and Ascendant','en');
INSERT INTO "APP_TEXTS" VALUES (110,'From both natal Moon and Ascendant','Від натального Місяця та Асцендента','uk');
INSERT INTO "APP_TEXTS" VALUES (111,'From both natal Moon and Ascendant','Od Księżyca i Ascendentu','pl');
INSERT INTO "APP_TEXTS" VALUES (112,'From both natal Moon and Ascendant','От натальной Луны и Асцендента','ru');
INSERT INTO "APP_TEXTS" VALUES (113,'Apply new settings for planetary transits display?','Apply new settings for planetary transits display?','en');
INSERT INTO "APP_TEXTS" VALUES (114,'Apply new settings for planetary transits display?','Застосувати нові налаштування відображення транзитів планет?','uk');
INSERT INTO "APP_TEXTS" VALUES (115,'Apply new settings for planetary transits display?','Zastosować nowe ustawienia wyświetlania tranzytów planetarnych?','pl');
INSERT INTO "APP_TEXTS" VALUES (116,'Apply new settings for planetary transits display?','Применить новые настройки отображения транзитов планет?','ru');
INSERT INTO "APP_TEXTS" VALUES (117,'Choose nodes settings for calculations:','Choose nodes settings for calculations:','en');
INSERT INTO "APP_TEXTS" VALUES (118,'Choose nodes settings for calculations:','Виберіть налаштування вузлів для розрахунків:','uk');
INSERT INTO "APP_TEXTS" VALUES (119,'Choose nodes settings for calculations:','Wybierz ustawienia węzłów do obliczeń:','pl');
INSERT INTO "APP_TEXTS" VALUES (120,'Choose nodes settings for calculations:','Выберите настройки узлов для расчётов:','ru');
INSERT INTO "APP_TEXTS" VALUES (121,'Mean nodes','Mean nodes','en');
INSERT INTO "APP_TEXTS" VALUES (122,'Mean nodes','Середні вузли','uk');
INSERT INTO "APP_TEXTS" VALUES (123,'Mean nodes','Średnie węzły','pl');
INSERT INTO "APP_TEXTS" VALUES (124,'Mean nodes','Средние узлы','ru');
INSERT INTO "APP_TEXTS" VALUES (125,'True nodes','True nodes','en');
INSERT INTO "APP_TEXTS" VALUES (126,'True nodes','Істинні вузли','uk');
INSERT INTO "APP_TEXTS" VALUES (127,'True nodes','Prawdziwe węzły','pl');
INSERT INTO "APP_TEXTS" VALUES (128,'True nodes','Истинные узлы','ru');
INSERT INTO "APP_TEXTS" VALUES (129,'Apply new settings for Rahu and Ketu calculations?','Apply new settings for Rahu and Ketu calculations?','en');
INSERT INTO "APP_TEXTS" VALUES (130,'Apply new settings for Rahu and Ketu calculations?','Застосувати нові налаштування для розрахунків Раху і Кету?','uk');
INSERT INTO "APP_TEXTS" VALUES (131,'Apply new settings for Rahu and Ketu calculations?','Zastosować nowe ustawienia dla obliczeń Rahu i Ketu?','pl');
INSERT INTO "APP_TEXTS" VALUES (132,'Apply new settings for Rahu and Ketu calculations?','Применить новые настройки для расчётов Раху и Кету?','ru');
INSERT INTO "APP_TEXTS" VALUES (133,'Choose Hora calculation mode:','Choose Hora calculation mode:','en');
INSERT INTO "APP_TEXTS" VALUES (134,'Choose Hora calculation mode:','Виберіть варіант розрахунку Хори:','uk');
INSERT INTO "APP_TEXTS" VALUES (135,'Choose Hora calculation mode:','Wybierz sposób obliczania Hory:','pl');
INSERT INTO "APP_TEXTS" VALUES (136,'Choose Hora calculation mode:','Выберите вариант расчета Хоры:','ru');
INSERT INTO "APP_TEXTS" VALUES (137,'From Sunrise to Sunset (1/12) + From Sunset to Sunrise (1/12)','From Sunrise to Sunset (1/12) + From Sunset to Sunrise (1/12)','en');
INSERT INTO "APP_TEXTS" VALUES (138,'From Sunrise to Sunset (1/12) + From Sunset to Sunrise (1/12)','Від Сходу до Заходу (1/12) + Від Заходу до Сходу (1/12)','uk');
INSERT INTO "APP_TEXTS" VALUES (139,'From Sunrise to Sunset (1/12) + From Sunset to Sunrise (1/12)','Od wschodu do zachodu (1/12) + od zachodu do wschodu (1/12)','pl');
INSERT INTO "APP_TEXTS" VALUES (140,'From Sunrise to Sunset (1/12) + From Sunset to Sunrise (1/12)','От Восхода до Заката (1/12) + От Заката до Восхода (1/12)','ru');
INSERT INTO "APP_TEXTS" VALUES (141,'From Sunrise to Sunrise (1/24)','From Sunrise to Sunrise (1/24)','en');
INSERT INTO "APP_TEXTS" VALUES (142,'From Sunrise to Sunrise (1/24)','Від Сходу до Сходу (1/24)','uk');
INSERT INTO "APP_TEXTS" VALUES (143,'From Sunrise to Sunrise (1/24)','Od wschodu do wschodu (1/24)','pl');
INSERT INTO "APP_TEXTS" VALUES (144,'From Sunrise to Sunrise (1/24)','От Восхода до Восхода (1/24)','ru');
INSERT INTO "APP_TEXTS" VALUES (145,'From 6:00 a.m. (Hora = 1 hour)','From 6:00 a.m. (Hora = 1 hour)','en');
INSERT INTO "APP_TEXTS" VALUES (146,'From 6:00 a.m. (Hora = 1 hour)','З 6:00 ранку (Хора = 1 година)','uk');
INSERT INTO "APP_TEXTS" VALUES (147,'From 6:00 a.m. (Hora = 1 hour)','Od 6:00 rano (Hora = 1 godzina)','pl');
INSERT INTO "APP_TEXTS" VALUES (148,'From 6:00 a.m. (Hora = 1 hour)','С 6:00 утра (Хора = 1 час)','ru');
INSERT INTO "APP_TEXTS" VALUES (149,'Apply new settings for Hora calculation?','Apply new settings for Hora calculation?','en');
INSERT INTO "APP_TEXTS" VALUES (150,'Apply new settings for Hora calculation?','Застосувати нові налаштування розрахунку Хори?','uk');
INSERT INTO "APP_TEXTS" VALUES (151,'Apply new settings for Hora calculation?','Zastosować nowe ustawienia obliczania Hory?','pl');
INSERT INTO "APP_TEXTS" VALUES (152,'Apply new settings for Hora calculation?','Применить новые настройки расчета Хоры?','ru');
INSERT INTO "APP_TEXTS" VALUES (153,'Choose how to calculate Muhurtas (Ghatis):','Choose how to calculate Muhurtas (Ghatis):','en');
INSERT INTO "APP_TEXTS" VALUES (154,'Choose how to calculate Muhurtas (Ghatis):','Виберіть варіант розрахунку Мухурт (Гхаті):','uk');
INSERT INTO "APP_TEXTS" VALUES (155,'Choose how to calculate Muhurtas (Ghatis):','Wybierz sposób obliczania Muhurt (Ghati):','pl');
INSERT INTO "APP_TEXTS" VALUES (156,'Choose how to calculate Muhurtas (Ghatis):','Выберите вариант расчета Мухурт (Гхати):','ru');
INSERT INTO "APP_TEXTS" VALUES (157,'From sunrise to sunset (1/15) + sunset to sunrise (1/15)','From sunrise to sunset (1/15) + sunset to sunrise (1/15)','en');
INSERT INTO "APP_TEXTS" VALUES (158,'From sunrise to sunset (1/15) + sunset to sunrise (1/15)','Від сходу до заходу (1/15) + від заходу до сходу (1/15)','uk');
INSERT INTO "APP_TEXTS" VALUES (159,'From sunrise to sunset (1/15) + sunset to sunrise (1/15)','Od wschodu do zachodu (1/15) + od zachodu do wschodu (1/15)','pl');
INSERT INTO "APP_TEXTS" VALUES (160,'From sunrise to sunset (1/15) + sunset to sunrise (1/15)','От восхода до заката (1/15) + от заката до восхода (1/15)','ru');
INSERT INTO "APP_TEXTS" VALUES (161,'From sunrise to sunrise (1/30)','From sunrise to sunrise (1/30)','en');
INSERT INTO "APP_TEXTS" VALUES (162,'From sunrise to sunrise (1/30)','Від сходу до сходу (1/30)','uk');
INSERT INTO "APP_TEXTS" VALUES (163,'From sunrise to sunrise (1/30)','Od wschodu do wschodu (1/30)','pl');
INSERT INTO "APP_TEXTS" VALUES (164,'From sunrise to sunrise (1/30)','От восхода до восхода (1/30)','ru');
INSERT INTO "APP_TEXTS" VALUES (165,'From 6:00 AM (Muhurta = 48 min)','From 6:00 AM (Muhurta = 48 min)','en');
INSERT INTO "APP_TEXTS" VALUES (166,'From 6:00 AM (Muhurta = 48 min)','З 6:00 ранку (Мухурта = 48 хв)','uk');
INSERT INTO "APP_TEXTS" VALUES (167,'From 6:00 AM (Muhurta = 48 min)','Od 6:00 rano (Muhurta = 48 min)','pl');
INSERT INTO "APP_TEXTS" VALUES (168,'From 6:00 AM (Muhurta = 48 min)','С 6:00 утра (Мухурта = 48 мин)','ru');
INSERT INTO "APP_TEXTS" VALUES (169,'Apply new settings for Muhurtas (Ghatis) calculation?','Apply new settings for Muhurtas (Ghatis) calculation?','en');
INSERT INTO "APP_TEXTS" VALUES (170,'Apply new settings for Muhurtas (Ghatis) calculation?','Застосувати нові налаштування розрахунку Мухурт (Гхаті)?','uk');
INSERT INTO "APP_TEXTS" VALUES (171,'Apply new settings for Muhurtas (Ghatis) calculation?','Zastosować nowe ustawienia obliczania Muhurt (Ghati)?','pl');
INSERT INTO "APP_TEXTS" VALUES (172,'Apply new settings for Muhurtas (Ghatis) calculation?','Применить новые настройки расчета Мухурт (Гхати)?','ru');
INSERT INTO "APP_TEXTS" VALUES (173,'Choose how to calculate Mrityu Bhaga:','Choose how to calculate Mrityu Bhaga:','en');
INSERT INTO "APP_TEXTS" VALUES (174,'Choose how to calculate Mrityu Bhaga:','Виберіть спосіб розрахунку Мритью Бхага:','uk');
INSERT INTO "APP_TEXTS" VALUES (175,'Choose how to calculate Mrityu Bhaga:','Wybierz metodę obliczania Mrityu Bhaga:','pl');
INSERT INTO "APP_TEXTS" VALUES (176,'Choose how to calculate Mrityu Bhaga:','Выберите способ расчета Мритью Бхага:','ru');
INSERT INTO "APP_TEXTS" VALUES (177,'From (N° - 30'') to (N° + 30'')','From (N° - 30'') to (N° + 30'')','en');
INSERT INTO "APP_TEXTS" VALUES (178,'From (N° - 30'') to (N° + 30'')','З (N° - 30'') до (N° + 30'')','uk');
INSERT INTO "APP_TEXTS" VALUES (179,'From (N° - 30'') to (N° + 30'')','Od (N° - 30'') do (N° + 30'')','pl');
INSERT INTO "APP_TEXTS" VALUES (180,'From (N° - 30'') to (N° + 30'')','С (N° - 30'') до (N° + 30'')','ru');
INSERT INTO "APP_TEXTS" VALUES (181,'From (N - 1)° to N°','From (N - 1)° to N°','en');
INSERT INTO "APP_TEXTS" VALUES (182,'From (N - 1)° to N°','З (N - 1)° до N°','uk');
INSERT INTO "APP_TEXTS" VALUES (183,'From (N - 1)° to N°','Od (N - 1)° do N°','pl');
INSERT INTO "APP_TEXTS" VALUES (184,'From (N - 1)° to N°','С (N - 1)° до N°','ru');
INSERT INTO "APP_TEXTS" VALUES (185,'From N° to (N + 1)°','From N° to (N + 1)°','en');
INSERT INTO "APP_TEXTS" VALUES (186,'From N° to (N + 1)°','З N° до (N + 1)°','uk');
INSERT INTO "APP_TEXTS" VALUES (187,'From N° to (N + 1)°','Od N° do (N + 1)°','pl');
INSERT INTO "APP_TEXTS" VALUES (188,'From N° to (N + 1)°','С N° до (N + 1)°','ru');
INSERT INTO "APP_TEXTS" VALUES (189,'From (N - 1)° to (N + 1)°','From (N - 1)° to (N + 1)°','en');
INSERT INTO "APP_TEXTS" VALUES (190,'From (N - 1)° to (N + 1)°','З (N - 1)° до (N + 1)°','uk');
INSERT INTO "APP_TEXTS" VALUES (191,'From (N - 1)° to (N + 1)°','Od (N - 1)° do (N + 1)°','pl');
INSERT INTO "APP_TEXTS" VALUES (192,'From (N - 1)° to (N + 1)°','С (N - 1)° до (N + 1)°','ru');
INSERT INTO "APP_TEXTS" VALUES (193,'Where ''N'' is the Mrityu Bhaga','Where ''N'' is the Mrityu Bhaga','en');
INSERT INTO "APP_TEXTS" VALUES (194,'Where ''N'' is the Mrityu Bhaga','Де "N" — Мритью Бхага','uk');
INSERT INTO "APP_TEXTS" VALUES (195,'Where ''N'' is the Mrityu Bhaga','Gdzie "N" — to Mrityu Bhaga','pl');
INSERT INTO "APP_TEXTS" VALUES (196,'Where ''N'' is the Mrityu Bhaga','Где "N" — Мритью Бхага','ru');
INSERT INTO "APP_TEXTS" VALUES (197,'Apply new settings for Mrityu Bhaga calculation?','Apply new settings for Mrityu Bhaga calculation?','en');
INSERT INTO "APP_TEXTS" VALUES (198,'Apply new settings for Mrityu Bhaga calculation?','Застосувати нові налаштування для розрахунку Мритью Бхага?','uk');
INSERT INTO "APP_TEXTS" VALUES (199,'Apply new settings for Mrityu Bhaga calculation?','Zastosować nowe ustawienia dla obliczania Mrityu Bhaga?','pl');
INSERT INTO "APP_TEXTS" VALUES (200,'Apply new settings for Mrityu Bhaga calculation?','Применить новые настройки расчета Мритью Бхага?','ru');
INSERT INTO "APP_TEXTS" VALUES (201,'Choose how to calculate the sunrise:','Choose how to calculate the sunrise:','en');
INSERT INTO "APP_TEXTS" VALUES (202,'Choose how to calculate the sunrise:','Виберіть спосіб розрахунку сходу Сонця:','uk');
INSERT INTO "APP_TEXTS" VALUES (203,'Choose how to calculate the sunrise:','Wybierz sposób obliczania wschodu słońca:','pl');
INSERT INTO "APP_TEXTS" VALUES (204,'Choose how to calculate the sunrise:','Выберите способ расчета восхода Солнца:','ru');
INSERT INTO "APP_TEXTS" VALUES (205,'Visible upper edge of the Sun disk','Visible upper edge of the Sun disk','en');
INSERT INTO "APP_TEXTS" VALUES (206,'Visible upper edge of the Sun disk','Видимий верхній край диска Сонця','uk');
INSERT INTO "APP_TEXTS" VALUES (207,'Visible upper edge of the Sun disk','Widoczna górna krawędź tarczy Słońca','pl');
INSERT INTO "APP_TEXTS" VALUES (208,'Visible upper edge of the Sun disk','Видимая верхняя кромка диска Солнца','ru');
INSERT INTO "APP_TEXTS" VALUES (209,'Visible center of the Sun disk','Visible center of the Sun disk','en');
INSERT INTO "APP_TEXTS" VALUES (210,'Visible center of the Sun disk','Видимий центр диска Сонця','uk');
INSERT INTO "APP_TEXTS" VALUES (211,'Visible center of the Sun disk','Widoczny środek tarczy Słońca','pl');
INSERT INTO "APP_TEXTS" VALUES (212,'Visible center of the Sun disk','Видимый центр диска Солнца','ru');
INSERT INTO "APP_TEXTS" VALUES (213,'Apply new settings for sunrise calculation?','Apply new settings for sunrise calculation?','en');
INSERT INTO "APP_TEXTS" VALUES (214,'Apply new settings for sunrise calculation?','Застосувати нові налаштування для розрахунку сходу Сонця?','uk');
INSERT INTO "APP_TEXTS" VALUES (215,'Apply new settings for sunrise calculation?','Zastosować nowe ustawienia obliczania wschodu słońca?','pl');
INSERT INTO "APP_TEXTS" VALUES (216,'Apply new settings for sunrise calculation?','Применить новые настройки расчета восхода Солнца?','ru');
INSERT INTO "APP_TEXTS" VALUES (217,'Profiles','Profiles','en');
INSERT INTO "APP_TEXTS" VALUES (218,'Profiles','Профілі','uk');
INSERT INTO "APP_TEXTS" VALUES (219,'Profiles','Profile','pl');
INSERT INTO "APP_TEXTS" VALUES (220,'Profiles','Профили','ru');
INSERT INTO "APP_TEXTS" VALUES (221,'Add new profile','Add new profile','en');
INSERT INTO "APP_TEXTS" VALUES (222,'Add new profile','Додати новий профіль','uk');
INSERT INTO "APP_TEXTS" VALUES (223,'Add new profile','Dodaj nowy profil','pl');
INSERT INTO "APP_TEXTS" VALUES (224,'Add new profile','Добавить новый профиль','ru');
INSERT INTO "APP_TEXTS" VALUES (225,'Profile','Profile','en');
INSERT INTO "APP_TEXTS" VALUES (226,'Profile','Профіль','uk');
INSERT INTO "APP_TEXTS" VALUES (227,'Profile','Profil','pl');
INSERT INTO "APP_TEXTS" VALUES (228,'Profile','Профиль','ru');
INSERT INTO "APP_TEXTS" VALUES (229,'Date and time of birth','Date and time of birth','en');
INSERT INTO "APP_TEXTS" VALUES (230,'Date and time of birth','Дата і час народження','uk');
INSERT INTO "APP_TEXTS" VALUES (231,'Date and time of birth','Data i godzina urodzenia','pl');
INSERT INTO "APP_TEXTS" VALUES (232,'Date and time of birth','Дата и время рождения','ru');
INSERT INTO "APP_TEXTS" VALUES (233,'Place of birth','Place of birth','en');
INSERT INTO "APP_TEXTS" VALUES (234,'Place of birth','Місце народження','uk');
INSERT INTO "APP_TEXTS" VALUES (235,'Place of birth','Miejsce urodzenia','pl');
INSERT INTO "APP_TEXTS" VALUES (236,'Place of birth','Место рождения','ru');
INSERT INTO "APP_TEXTS" VALUES (237,'Place of living','Place of living','en');
INSERT INTO "APP_TEXTS" VALUES (238,'Place of living','Місце проживання','uk');
INSERT INTO "APP_TEXTS" VALUES (239,'Place of living','Miejsce zamieszkania','pl');
INSERT INTO "APP_TEXTS" VALUES (240,'Place of living','Место проживания','ru');
INSERT INTO "APP_TEXTS" VALUES (241,'Profile name','Profile name','en');
INSERT INTO "APP_TEXTS" VALUES (242,'Profile name','Назва профілю','uk');
INSERT INTO "APP_TEXTS" VALUES (243,'Profile name','Nazwa profilu','pl');
INSERT INTO "APP_TEXTS" VALUES (244,'Profile name','Имя профиля','ru');
INSERT INTO "APP_TEXTS" VALUES (245,'First name','First name','en');
INSERT INTO "APP_TEXTS" VALUES (246,'First name','Ім’я','uk');
INSERT INTO "APP_TEXTS" VALUES (247,'First name','Imię','pl');
INSERT INTO "APP_TEXTS" VALUES (248,'First name','Имя','ru');
INSERT INTO "APP_TEXTS" VALUES (249,'Last name','Last name','en');
INSERT INTO "APP_TEXTS" VALUES (250,'Last name','Прізвище','uk');
INSERT INTO "APP_TEXTS" VALUES (251,'Last name','Nazwisko','pl');
INSERT INTO "APP_TEXTS" VALUES (252,'Last name','Фамилия','ru');
INSERT INTO "APP_TEXTS" VALUES (253,'Notes','Notes','en');
INSERT INTO "APP_TEXTS" VALUES (254,'Notes','Нотатки','uk');
INSERT INTO "APP_TEXTS" VALUES (255,'Notes','Notatki','pl');
INSERT INTO "APP_TEXTS" VALUES (256,'Notes','Заметки','ru');
INSERT INTO "APP_TEXTS" VALUES (257,'Select location...','Select location...','en');
INSERT INTO "APP_TEXTS" VALUES (258,'Select location...','Виберіть місце...','uk');
INSERT INTO "APP_TEXTS" VALUES (259,'Select location...','Wybierz lokalizację...','pl');
INSERT INTO "APP_TEXTS" VALUES (260,'Select location...','Выберите место...','ru');
INSERT INTO "APP_TEXTS" VALUES (261,'Save changes','Save changes','en');
INSERT INTO "APP_TEXTS" VALUES (262,'Save changes','Зберегти зміни','uk');
INSERT INTO "APP_TEXTS" VALUES (263,'Save changes','Zapisz zmiany','pl');
INSERT INTO "APP_TEXTS" VALUES (264,'Save changes','Сохранить изменения','ru');
INSERT INTO "APP_TEXTS" VALUES (265,'Do you want to save changes before exit?','Do you want to save changes before exit?','en');
INSERT INTO "APP_TEXTS" VALUES (266,'Do you want to save changes before exit?','Зберегти зміни перед виходом?','uk');
INSERT INTO "APP_TEXTS" VALUES (267,'Do you want to save changes before exit?','Czy chcesz zapisać zmiany przed wyjściem?','pl');
INSERT INTO "APP_TEXTS" VALUES (268,'Do you want to save changes before exit?','Сохранить изменения перед выходом?','ru');
INSERT INTO "APP_TEXTS" VALUES (269,'Validation','Validation','en');
INSERT INTO "APP_TEXTS" VALUES (270,'Validation','Перевірка','uk');
INSERT INTO "APP_TEXTS" VALUES (271,'Validation','Walidacja','pl');
INSERT INTO "APP_TEXTS" VALUES (272,'Validation','Проверка','ru');
INSERT INTO "APP_TEXTS" VALUES (273,'Profile name is required.','Profile name is required.','en');
INSERT INTO "APP_TEXTS" VALUES (274,'Profile name is required.','Необхідно вказати назву профілю.','uk');
INSERT INTO "APP_TEXTS" VALUES (275,'Profile name is required.','Nazwa profilu jest wymagana.','pl');
INSERT INTO "APP_TEXTS" VALUES (276,'Profile name is required.','Необходимо указать имя профиля.','ru');
INSERT INTO "APP_TEXTS" VALUES (277,'Date of birth is required.','Date of birth is required.','en');
INSERT INTO "APP_TEXTS" VALUES (278,'Date of birth is required.','Необхідно вказати дату народження.','uk');
INSERT INTO "APP_TEXTS" VALUES (279,'Date of birth is required.','Data urodzenia jest wymagana.','pl');
INSERT INTO "APP_TEXTS" VALUES (280,'Date of birth is required.','Необходимо указать дату рождения.','ru');
INSERT INTO "APP_TEXTS" VALUES (281,'Place of birth is required.','Place of birth is required.','en');
INSERT INTO "APP_TEXTS" VALUES (282,'Place of birth is required.','Необхідно вказати місце народження.','uk');
INSERT INTO "APP_TEXTS" VALUES (283,'Place of birth is required.','Miejsce urodzenia jest wymagane.','pl');
INSERT INTO "APP_TEXTS" VALUES (284,'Place of birth is required.','Необходимо указать место рождения.','ru');
INSERT INTO "APP_TEXTS" VALUES (285,'Place of living is required.','Place of living is required.','en');
INSERT INTO "APP_TEXTS" VALUES (286,'Place of living is required.','Необхідно вказати місце проживання.','uk');
INSERT INTO "APP_TEXTS" VALUES (287,'Place of living is required.','Miejsce zamieszkania jest wymagane.','pl');
INSERT INTO "APP_TEXTS" VALUES (288,'Place of living is required.','Необходимо указать место проживания.','ru');
INSERT INTO "APP_TEXTS" VALUES (289,'Saved','Saved','en');
INSERT INTO "APP_TEXTS" VALUES (290,'Saved','Збережено','uk');
INSERT INTO "APP_TEXTS" VALUES (291,'Saved','Zapisano','pl');
INSERT INTO "APP_TEXTS" VALUES (292,'Saved','Сохранено','ru');
INSERT INTO "APP_TEXTS" VALUES (293,'Profile saved successfully.','Profile saved successfully.','en');
INSERT INTO "APP_TEXTS" VALUES (294,'Profile saved successfully.','Профіль успішно збережено.','uk');
INSERT INTO "APP_TEXTS" VALUES (295,'Profile saved successfully.','Profil został pomyślnie zapisany.','pl');
INSERT INTO "APP_TEXTS" VALUES (296,'Profile saved successfully.','Профиль успешно сохранён.','ru');
INSERT INTO "APP_TEXTS" VALUES (297,'Error','Error','en');
INSERT INTO "APP_TEXTS" VALUES (298,'Error','Помилка','uk');
INSERT INTO "APP_TEXTS" VALUES (299,'Error','Błąd','pl');
INSERT INTO "APP_TEXTS" VALUES (300,'Error','Ошибка','ru');
INSERT INTO "APP_TEXTS" VALUES (301,'Failed to save profile. Please try again.','Failed to save profile. Please try again.','en');
INSERT INTO "APP_TEXTS" VALUES (302,'Failed to save profile. Please try again.','Не вдалося зберегти профіль. Спробуйте ще раз.','uk');
INSERT INTO "APP_TEXTS" VALUES (303,'Failed to save profile. Please try again.','Nie udało się zapisać profilu. Spróbuj ponownie.','pl');
INSERT INTO "APP_TEXTS" VALUES (304,'Failed to save profile. Please try again.','Не удалось сохранить профиль. Попробуйте ещё раз.','ru');
INSERT INTO "APP_TEXTS" VALUES (305,'Default profile','Default profile','en');
INSERT INTO "APP_TEXTS" VALUES (306,'Default profile','Профіль за замовчуванням','uk');
INSERT INTO "APP_TEXTS" VALUES (307,'Default profile','Domyślny profil','pl');
INSERT INTO "APP_TEXTS" VALUES (308,'Default profile','Профиль по умолчанию','ru');
INSERT INTO "APP_TEXTS" VALUES (309,'Save profile first.','Save profile first.','en');
INSERT INTO "APP_TEXTS" VALUES (310,'Save profile first.','Спочатку збережіть профіль.','uk');
INSERT INTO "APP_TEXTS" VALUES (311,'Save profile first.','Najpierw zapisz profil.','pl');
INSERT INTO "APP_TEXTS" VALUES (312,'Save profile first.','Сначала сохраните профиль.','ru');
INSERT INTO "APP_TEXTS" VALUES (313,'Set this profile as default?','Set this profile as default?','en');
INSERT INTO "APP_TEXTS" VALUES (314,'Set this profile as default?','Зробити цей профіль основним?','uk');
INSERT INTO "APP_TEXTS" VALUES (315,'Set this profile as default?','Ustawić ten profil jako domyślny?','pl');
INSERT INTO "APP_TEXTS" VALUES (316,'Set this profile as default?','Сделать этот профиль основным?','ru');
INSERT INTO "APP_TEXTS" VALUES (317,'Done','Done','en');
INSERT INTO "APP_TEXTS" VALUES (318,'Done','Готово','uk');
INSERT INTO "APP_TEXTS" VALUES (319,'Done','Gotowe','pl');
INSERT INTO "APP_TEXTS" VALUES (320,'Done','Готово','ru');
INSERT INTO "APP_TEXTS" VALUES (321,'Profile marked as default.','Profile marked as default.','en');
INSERT INTO "APP_TEXTS" VALUES (322,'Profile marked as default.','Профіль позначено як основний.','uk');
INSERT INTO "APP_TEXTS" VALUES (323,'Profile marked as default.','Profil ustawiony jako domyślny.','pl');
INSERT INTO "APP_TEXTS" VALUES (324,'Profile marked as default.','Профиль отмечен как основной.','ru');
INSERT INTO "APP_TEXTS" VALUES (325,'Delete','Delete','en');
INSERT INTO "APP_TEXTS" VALUES (326,'Delete','Видалити','uk');
INSERT INTO "APP_TEXTS" VALUES (327,'Delete','Usuń','pl');
INSERT INTO "APP_TEXTS" VALUES (328,'Delete','Удалить','ru');
INSERT INTO "APP_TEXTS" VALUES (329,'Delete this profile?','Delete this profile?','en');
INSERT INTO "APP_TEXTS" VALUES (330,'Delete this profile?','Видалити цей профіль?','uk');
INSERT INTO "APP_TEXTS" VALUES (331,'Delete this profile?','Usunąć ten profil?','pl');
INSERT INTO "APP_TEXTS" VALUES (332,'Delete this profile?','Удалить этот профиль?','ru');
INSERT INTO "APP_TEXTS" VALUES (333,'Deleted','Deleted','en');
INSERT INTO "APP_TEXTS" VALUES (334,'Deleted','Видалено','uk');
INSERT INTO "APP_TEXTS" VALUES (335,'Deleted','Usunięto','pl');
INSERT INTO "APP_TEXTS" VALUES (336,'Deleted','Удалено','ru');
INSERT INTO "APP_TEXTS" VALUES (337,'Profile deleted.','Profile deleted.','en');
INSERT INTO "APP_TEXTS" VALUES (338,'Profile deleted.','Профіль видалено.','uk');
INSERT INTO "APP_TEXTS" VALUES (339,'Profile deleted.','Profil został usunięty.','pl');
INSERT INTO "APP_TEXTS" VALUES (340,'Profile deleted.','Профиль удалён.','ru');
INSERT INTO "APP_TEXTS" VALUES (341,'Location search','Location search','en');
INSERT INTO "APP_TEXTS" VALUES (342,'Location search','Пошук місця','uk');
INSERT INTO "APP_TEXTS" VALUES (343,'Location search','Wyszukiwanie lokalizacji','pl');
INSERT INTO "APP_TEXTS" VALUES (344,'Location search','Поиск местоположения','ru');
INSERT INTO "APP_TEXTS" VALUES (345,'Enter city name','Enter city name','en');
INSERT INTO "APP_TEXTS" VALUES (346,'Enter city name','Введіть назву міста','uk');
INSERT INTO "APP_TEXTS" VALUES (347,'Enter city name','Wpisz nazwę miasta','pl');
INSERT INTO "APP_TEXTS" VALUES (348,'Enter city name','Введите название города','ru');
INSERT INTO "APP_TEXTS" VALUES (349,'Select','Select','en');
INSERT INTO "APP_TEXTS" VALUES (350,'Select','Вибрати','uk');
INSERT INTO "APP_TEXTS" VALUES (351,'Select','Wybierz','pl');
INSERT INTO "APP_TEXTS" VALUES (352,'Select','Выбрать','ru');
INSERT INTO "APP_TEXTS" VALUES (353,'Save','Save','en');
INSERT INTO "APP_TEXTS" VALUES (354,'Save','Зберегти','uk');
INSERT INTO "APP_TEXTS" VALUES (355,'Save','Zapisz','pl');
INSERT INTO "APP_TEXTS" VALUES (356,'Save','Сохранить','ru');
INSERT INTO "APP_TEXTS" VALUES (357,'Save changes to profile?','Save changes to profile?','en');
INSERT INTO "APP_TEXTS" VALUES (358,'Save changes to profile?','Зберегти зміни до профілю?','uk');
INSERT INTO "APP_TEXTS" VALUES (359,'Save changes to profile?','Zapisać zmiany w profilu?','pl');
INSERT INTO "APP_TEXTS" VALUES (360,'Save changes to profile?','Сохранить изменения в профиле?','ru');
INSERT INTO "COLOR" VALUES (1,'GREEN',-13631697);
INSERT INTO "COLOR" VALUES (2,'RED',-45233);
INSERT INTO "COLOR" VALUES (3,'LIGHTGREEN',-4587591);
INSERT INTO "COLOR" VALUES (4,'LIGHTRED',-14650);
INSERT INTO "COLOR" VALUES (5,'PINK',-16181);
INSERT INTO "COLOR" VALUES (6,'JOGAMERGE',-3211314);
INSERT INTO "COLOR" VALUES (7,'MUHURTAMERGE',-16181);
INSERT INTO "COLOR" VALUES (8,'SELECTRECTANGLE',-16776961);
INSERT INTO "COLOR" VALUES (9,'SUN',-4587591);
INSERT INTO "COLOR" VALUES (10,'VENUS',-4587591);
INSERT INTO "COLOR" VALUES (11,'MERCURY',-4587591);
INSERT INTO "COLOR" VALUES (12,'MOON',-4587591);
INSERT INTO "COLOR" VALUES (13,'SATURN',-14650);
INSERT INTO "COLOR" VALUES (14,'JUPITER',-4587591);
INSERT INTO "COLOR" VALUES (15,'MARS',-14650);
INSERT INTO "COLOR" VALUES (16,'MASA1',-4587591);
INSERT INTO "COLOR" VALUES (17,'MASA2',-4587591);
INSERT INTO "COLOR" VALUES (18,'MASA3',-4587591);
INSERT INTO "COLOR" VALUES (19,'MASA4',-4587591);
INSERT INTO "COLOR" VALUES (20,'MASA5',-4587591);
INSERT INTO "COLOR" VALUES (21,'MASA6',-4587591);
INSERT INTO "COLOR" VALUES (22,'MASA7',-4587591);
INSERT INTO "COLOR" VALUES (23,'MASA8',-4587591);
INSERT INTO "COLOR" VALUES (24,'MASA9',-4587591);
INSERT INTO "COLOR" VALUES (25,'MASA10',-4587591);
INSERT INTO "COLOR" VALUES (26,'MASA11',-4587591);
INSERT INTO "COLOR" VALUES (27,'MASA12',-4587591);
INSERT INTO "COLOR" VALUES (28,'SHUNYANAKSHATRA',-16181);
INSERT INTO "COLOR" VALUES (29,'SHUNIATITHI',-16181);
INSERT INTO "COLOR" VALUES (30,'MRITYUBHAGA',-1064908);
INSERT INTO "COLOR" VALUES (31,'GRAY',-4144960);
INSERT INTO "COLOR" VALUES (32,'BLACK',-16777216);
INSERT INTO "COLOR_DESC" VALUES (1,1,'Green','en');
INSERT INTO "COLOR_DESC" VALUES (2,2,'Red','en');
INSERT INTO "COLOR_DESC" VALUES (3,3,'Light green','en');
INSERT INTO "COLOR_DESC" VALUES (4,4,'Light red','en');
INSERT INTO "COLOR_DESC" VALUES (5,5,'Pink','en');
INSERT INTO "COLOR_DESC" VALUES (6,6,'Yoga merge','en');
INSERT INTO "COLOR_DESC" VALUES (7,7,'Muhurta merge','en');
INSERT INTO "COLOR_DESC" VALUES (8,8,'Selection rectangle','en');
INSERT INTO "COLOR_DESC" VALUES (9,9,'Hora: Sun','en');
INSERT INTO "COLOR_DESC" VALUES (10,10,'Hora: Venus','en');
INSERT INTO "COLOR_DESC" VALUES (11,11,'Hora: Mercury','en');
INSERT INTO "COLOR_DESC" VALUES (12,12,'Hora: Moon','en');
INSERT INTO "COLOR_DESC" VALUES (13,13,'Hora: Saturn','en');
INSERT INTO "COLOR_DESC" VALUES (14,14,'Hora: Jupiter','en');
INSERT INTO "COLOR_DESC" VALUES (15,15,'Hora: Mars','en');
INSERT INTO "COLOR_DESC" VALUES (16,16,'Masa: Chaitra','en');
INSERT INTO "COLOR_DESC" VALUES (17,17,'Masa: Vaisakha','en');
INSERT INTO "COLOR_DESC" VALUES (18,18,'Masa: Jyeshtha','en');
INSERT INTO "COLOR_DESC" VALUES (19,19,'Masa: Ashadha','en');
INSERT INTO "COLOR_DESC" VALUES (20,20,'Masa: Shravana','en');
INSERT INTO "COLOR_DESC" VALUES (21,21,'Masa: Bhadrapada','en');
INSERT INTO "COLOR_DESC" VALUES (22,22,'Masa: Ashvina','en');
INSERT INTO "COLOR_DESC" VALUES (23,23,'Masa: Kartika','en');
INSERT INTO "COLOR_DESC" VALUES (24,24,'Masa: Margasira','en');
INSERT INTO "COLOR_DESC" VALUES (25,25,'Masa: Pushya','en');
INSERT INTO "COLOR_DESC" VALUES (26,26,'Masa: Magha','en');
INSERT INTO "COLOR_DESC" VALUES (27,27,'Masa: Phalguna','en');
INSERT INTO "COLOR_DESC" VALUES (28,28,'Shunya: Nakshatra','en');
INSERT INTO "COLOR_DESC" VALUES (29,29,'Shunya: Tithi','en');
INSERT INTO "COLOR_DESC" VALUES (30,30,'Mrityu Bhaga','en');
INSERT INTO "COLOR_DESC" VALUES (31,31,'Gray','en');
INSERT INTO "COLOR_DESC" VALUES (32,32,'Black','en');
INSERT INTO "COLOR_DESC" VALUES (33,1,'Зеленый','ru');
INSERT INTO "COLOR_DESC" VALUES (34,2,'Красный','ru');
INSERT INTO "COLOR_DESC" VALUES (35,3,'Светло зеленый','ru');
INSERT INTO "COLOR_DESC" VALUES (36,4,'Светло красный','ru');
INSERT INTO "COLOR_DESC" VALUES (37,5,'Розовый','ru');
INSERT INTO "COLOR_DESC" VALUES (38,6,'Наложение йог','ru');
INSERT INTO "COLOR_DESC" VALUES (39,7,'Наложение мухурт','ru');
INSERT INTO "COLOR_DESC" VALUES (40,8,'Рамка выбора','ru');
INSERT INTO "COLOR_DESC" VALUES (41,9,'Хора: Солнце','ru');
INSERT INTO "COLOR_DESC" VALUES (42,10,'Хора: Венера','ru');
INSERT INTO "COLOR_DESC" VALUES (43,11,'Хора: Меркурий','ru');
INSERT INTO "COLOR_DESC" VALUES (44,12,'Хора: Луна','ru');
INSERT INTO "COLOR_DESC" VALUES (45,13,'Хора: Сатурн','ru');
INSERT INTO "COLOR_DESC" VALUES (46,14,'Хора: Юпитер','ru');
INSERT INTO "COLOR_DESC" VALUES (47,15,'Хора: Марс','ru');
INSERT INTO "COLOR_DESC" VALUES (48,16,'Маса: Чаитра','ru');
INSERT INTO "COLOR_DESC" VALUES (49,17,'Маса: Вайшакха','ru');
INSERT INTO "COLOR_DESC" VALUES (50,18,'Маса: Джаештха','ru');
INSERT INTO "COLOR_DESC" VALUES (51,19,'Маса: Ашадха','ru');
INSERT INTO "COLOR_DESC" VALUES (52,20,'Маса: Шравана','ru');
INSERT INTO "COLOR_DESC" VALUES (53,21,'Маса: Бхадрапада','ru');
INSERT INTO "COLOR_DESC" VALUES (54,22,'Маса: Ашвина','ru');
INSERT INTO "COLOR_DESC" VALUES (55,23,'Маса: Картика','ru');
INSERT INTO "COLOR_DESC" VALUES (56,24,'Маса: Маргашира','ru');
INSERT INTO "COLOR_DESC" VALUES (57,25,'Маса: Пушья','ru');
INSERT INTO "COLOR_DESC" VALUES (58,26,'Маса: Магха','ru');
INSERT INTO "COLOR_DESC" VALUES (59,27,'Маса: Пхалгуна','ru');
INSERT INTO "COLOR_DESC" VALUES (60,28,'Шунья: Накшатра','ru');
INSERT INTO "COLOR_DESC" VALUES (61,29,'Шунья: Титхи','ru');
INSERT INTO "COLOR_DESC" VALUES (62,30,'Мритью Бхага','ru');
INSERT INTO "COLOR_DESC" VALUES (63,31,'Серый','ru');
INSERT INTO "COLOR_DESC" VALUES (64,32,'Черный','ru');
INSERT INTO "DVLINENAMES" VALUES (1,'MUHURTA');
INSERT INTO "DVLINENAMES" VALUES (2,'YOGA');
INSERT INTO "DVLINENAMES" VALUES (3,'NAKSHATRA');
INSERT INTO "DVLINENAMES" VALUES (4,'TARABALA');
INSERT INTO "DVLINENAMES" VALUES (5,'TITHI');
INSERT INTO "DVLINENAMES" VALUES (6,'KARANA');
INSERT INTO "DVLINENAMES" VALUES (7,'NITYAYOGA');
INSERT INTO "DVLINENAMES" VALUES (8,'CHANDRABALA');
INSERT INTO "DVLINENAMES" VALUES (9,'HORA');
INSERT INTO "DVLINENAMES" VALUES (10,'MUHURTA30');
INSERT INTO "DVLINENAMES" VALUES (11,'GHTATI60');
INSERT INTO "DVLINENAMES" VALUES (12,'MOONPADA');
INSERT INTO "DVLINENAMES" VALUES (13,'SUNPADA');
INSERT INTO "DVLINENAMES" VALUES (14,'VENUSPADA');
INSERT INTO "DVLINENAMES" VALUES (15,'JUPITERPADA');
INSERT INTO "DVLINENAMES" VALUES (16,'MERCURYPADA');
INSERT INTO "DVLINENAMES" VALUES (17,'MARSPADA');
INSERT INTO "DVLINENAMES" VALUES (18,'SATURNPADA');
INSERT INTO "DVLINENAMES" VALUES (19,'RAHUPADA');
INSERT INTO "DVLINENAMES" VALUES (20,'KETUPADA');
INSERT INTO "DVLINENAMES_DESC" VALUES (1,1,'Mu','Muhurta','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (2,2,'Yo','Yoga','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (3,3,'Na','Nakshatra','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (4,4,'TB','Tara Bala','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (5,5,'Ti','Tithi','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (6,6,'Ka','Karana','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (7,7,'NY','Nitya Yoga','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (8,8,'CB','Chandra Bala','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (9,9,'Ho','Hora','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (10,10,'30M','30 Muhurts','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (11,11,'60G','60 Ghati','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (12,12,'MoP','Moon','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (13,13,'SuP','Sun','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (14,14,'VeP','Venus','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (15,15,'JuP','Jupiter','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (16,16,'MeP','Mercury','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (17,17,'MaP','Mars','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (18,18,'SaP','Saturn','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (19,19,'RaP','Rahu','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (20,20,'KeP','Ketu','en');
INSERT INTO "DVLINENAMES_DESC" VALUES (21,1,'Му','Мухурта','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (22,2,'Йо','Йога','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (23,3,'На','Накшатра','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (24,4,'ТБ','Тара Бала','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (25,5,'Ти','Титхи','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (26,6,'Ка','Карана','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (27,7,'НЙ','Нитья Йога','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (28,8,'ЧБ','Чандра Бала','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (29,9,'Хо','Хора','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (30,10,'30М','30 Мухурт','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (31,11,'60Г','60 Гхати','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (32,12,'ЛуП','Луна','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (33,13,'СоП','Солнце','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (34,14,'ВеП','Венера','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (35,15,'ЮпП','Юпитер','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (36,16,'МеП','Меркурий','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (37,17,'МаП','Марс','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (38,18,'СаП','Сатурн','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (39,19,'РаП','Раху','ru');
INSERT INTO "DVLINENAMES_DESC" VALUES (40,20,'КеП','Кету','ru');
INSERT INTO "ECLIPSE" VALUES (1,'MOONECLIPSE');
INSERT INTO "ECLIPSE" VALUES (2,'SUNECLIPSE');
INSERT INTO "ECLIPSE_DESC" VALUES (1,1,'Moon Eclipse','en');
INSERT INTO "ECLIPSE_DESC" VALUES (2,1,'Місячне затемнення','uk');
INSERT INTO "ECLIPSE_DESC" VALUES (3,1,'Zaćmienie Księżyca','pl');
INSERT INTO "ECLIPSE_DESC" VALUES (4,1,'Лунное затмение','ru');
INSERT INTO "ECLIPSE_DESC" VALUES (5,2,'Sun Eclipse','en');
INSERT INTO "ECLIPSE_DESC" VALUES (6,2,'Сонячне затемнення','uk');
INSERT INTO "ECLIPSE_DESC" VALUES (7,2,'Zaćmienie Słońca','pl');
INSERT INTO "ECLIPSE_DESC" VALUES (8,2,'Солнечное затмение','ru');
INSERT INTO "FONTLIST" VALUES (1,12,'HEADER',2);
INSERT INTO "FONTLIST" VALUES (2,13,'CALENDARTEXT',1);
INSERT INTO "FONTLIST" VALUES (3,13,'TRANZITTEXT',1);
INSERT INTO "FONTLIST" VALUES (4,7,'TRANSTOOLTIPHEADER',2);
INSERT INTO "FONTLIST" VALUES (5,7,'TRANSTOOLTIPTEXT',3);
INSERT INTO "FONTLIST" VALUES (6,11,'DWTOOLTIPTITLE',2);
INSERT INTO "FONTLIST" VALUES (7,11,'DWTOOLTIPTIME',1);
INSERT INTO "FONTLIST" VALUES (8,11,'DWTOOLTIPTEXT',3);
INSERT INTO "FONTLIST" VALUES (9,11,'PEVTOOLTIPDATE',2);
INSERT INTO "FONTLIST" VALUES (10,11,'PEVTOOLTIPTIME',1);
INSERT INTO "FONTLIST" VALUES (11,4,'PEVTOOLTIPTEXT',3);
INSERT INTO "FONTLIST_DESC" VALUES (1,1,'Header','en');
INSERT INTO "FONTLIST_DESC" VALUES (2,2,'Calendar text','en');
INSERT INTO "FONTLIST_DESC" VALUES (3,3,'Tranzit text','en');
INSERT INTO "FONTLIST_DESC" VALUES (4,4,'Tranzit tooltip header','en');
INSERT INTO "FONTLIST_DESC" VALUES (5,5,'Tranzit tooltip text','en');
INSERT INTO "FONTLIST_DESC" VALUES (6,6,'Diary tooltip title','en');
INSERT INTO "FONTLIST_DESC" VALUES (7,7,'Diary tooltip time period','en');
INSERT INTO "FONTLIST_DESC" VALUES (8,8,'Diary tooltip text','en');
INSERT INTO "FONTLIST_DESC" VALUES (9,9,'Appointment tooltip date','en');
INSERT INTO "FONTLIST_DESC" VALUES (10,10,'Appointment tooltip time','en');
INSERT INTO "FONTLIST_DESC" VALUES (11,11,'Appointment tooltip text','en');
INSERT INTO "FONTLIST_DESC" VALUES (12,1,'Заголовок календаря','ru');
INSERT INTO "FONTLIST_DESC" VALUES (13,2,'Текст календаря','ru');
INSERT INTO "FONTLIST_DESC" VALUES (14,3,'Текст транзитов','ru');
INSERT INTO "FONTLIST_DESC" VALUES (15,4,'Заголовок подсказки транзитов','ru');
INSERT INTO "FONTLIST_DESC" VALUES (16,5,'Текст подсказки транзитов','ru');
INSERT INTO "FONTLIST_DESC" VALUES (17,6,'Заголовок подсказки ежедневника','ru');
INSERT INTO "FONTLIST_DESC" VALUES (18,7,'Временной период подсказки ежедневника','ru');
INSERT INTO "FONTLIST_DESC" VALUES (19,8,'Текст подсказки ежедневника','ru');
INSERT INTO "FONTLIST_DESC" VALUES (20,9,'Дата подсказки записи','ru');
INSERT INTO "FONTLIST_DESC" VALUES (21,10,'Время подсказки записи','ru');
INSERT INTO "FONTLIST_DESC" VALUES (22,11,'Текст подсказки записи','ru');
INSERT INTO "GHATI60" VALUES (1,1,2,'GHORA');
INSERT INTO "GHATI60" VALUES (2,1,2,'RAKSHAS');
INSERT INTO "GHATI60" VALUES (3,2,2,'NAGA');
INSERT INTO "GHATI60" VALUES (4,2,1,'KUBERA');
INSERT INTO "GHATI60" VALUES (5,3,2,'YAKSHA');
INSERT INTO "GHATI60" VALUES (6,3,1,'KINARA');
INSERT INTO "GHATI60" VALUES (7,4,2,'BHRASHTRA');
INSERT INTO "GHATI60" VALUES (8,4,2,'KULAGNA');
INSERT INTO "GHATI60" VALUES (9,5,2,'VISHA');
INSERT INTO "GHATI60" VALUES (10,5,2,'AGNI');
INSERT INTO "GHATI60" VALUES (11,6,2,'MAYA');
INSERT INTO "GHATI60" VALUES (12,6,2,'PRETAPURISHA');
INSERT INTO "GHATI60" VALUES (13,7,1,'APAMPATI');
INSERT INTO "GHATI60" VALUES (14,7,1,'MARUTVANA');
INSERT INTO "GHATI60" VALUES (15,8,1,'KALA');
INSERT INTO "GHATI60" VALUES (16,8,1,'SHESHA');
INSERT INTO "GHATI60" VALUES (17,9,1,'AVAMRITA');
INSERT INTO "GHATI60" VALUES (18,9,1,'SOMA');
INSERT INTO "GHATI60" VALUES (19,10,1,'MRIDVA');
INSERT INTO "GHATI60" VALUES (20,10,1,'KOMALA');
INSERT INTO "GHATI60" VALUES (21,11,1,'PADMA');
INSERT INTO "GHATI60" VALUES (22,11,1,'BRAHMA');
INSERT INTO "GHATI60" VALUES (23,12,2,'DISHTA');
INSERT INTO "GHATI60" VALUES (24,12,2,'DIGAMBARA');
INSERT INTO "GHATI60" VALUES (25,13,1,'DEVA');
INSERT INTO "GHATI60" VALUES (26,13,2,'ARDRA');
INSERT INTO "GHATI60" VALUES (27,14,2,'KALINASHA');
INSERT INTO "GHATI60" VALUES (28,14,1,'KSHITISHWARA');
INSERT INTO "GHATI60" VALUES (29,15,1,'KAMALAKARA');
INSERT INTO "GHATI60" VALUES (30,15,2,'MANDAMAJA');
INSERT INTO "GHATI60" VALUES (31,16,2,'MRITYU');
INSERT INTO "GHATI60" VALUES (32,16,2,'ASITA');
INSERT INTO "GHATI60" VALUES (33,17,2,'DAVAGNI');
INSERT INTO "GHATI60" VALUES (34,17,2,'SAMSAYA');
INSERT INTO "GHATI60" VALUES (35,18,2,'YAMA');
INSERT INTO "GHATI60" VALUES (36,18,2,'KANTAKA');
INSERT INTO "GHATI60" VALUES (37,19,1,'SUDHA');
INSERT INTO "GHATI60" VALUES (38,19,1,'AMRITA38');
INSERT INTO "GHATI60" VALUES (39,20,1,'PURNACHANDRA');
INSERT INTO "GHATI60" VALUES (40,20,2,'VISHAPRADAGDHA');
INSERT INTO "GHATI60" VALUES (41,21,2,'KULANASHA');
INSERT INTO "GHATI60" VALUES (42,21,1,'VAMSHAKSYA');
INSERT INTO "GHATI60" VALUES (43,22,1,'UTPATAKA');
INSERT INTO "GHATI60" VALUES (44,22,2,'RUPA');
INSERT INTO "GHATI60" VALUES (45,23,1,'SAUMYA');
INSERT INTO "GHATI60" VALUES (46,23,1,'MRIDU');
INSERT INTO "GHATI60" VALUES (47,24,1,'SHITALA');
INSERT INTO "GHATI60" VALUES (48,24,1,'KADAMSHTRA');
INSERT INTO "GHATI60" VALUES (49,25,1,'INDUMUKHA');
INSERT INTO "GHATI60" VALUES (50,25,1,'PRAVINA');
INSERT INTO "GHATI60" VALUES (51,26,2,'KALABALA');
INSERT INTO "GHATI60" VALUES (52,26,2,'DANDA');
INSERT INTO "GHATI60" VALUES (53,27,1,'NIRMALA');
INSERT INTO "GHATI60" VALUES (54,27,1,'SHUBHA');
INSERT INTO "GHATI60" VALUES (55,28,2,'ASHUBHA');
INSERT INTO "GHATI60" VALUES (56,28,2,'DALA');
INSERT INTO "GHATI60" VALUES (57,29,1,'AMRITA57');
INSERT INTO "GHATI60" VALUES (58,29,1,'PAYODHIVIDYA');
INSERT INTO "GHATI60" VALUES (59,30,2,'BHRAMA');
INSERT INTO "GHATI60" VALUES (60,30,2,'REKHA');
INSERT INTO "GHATI60_DESC" VALUES (1,1,'Ghora','1st GHATI GHORA','Lord of horror, respect, shock, magical influences','en');
INSERT INTO "GHATI60_DESC" VALUES (2,2,'Rakshas','2nd GHATI RAKSHAS','Lord of the guards, guards, harm, destruction and trials','en');
INSERT INTO "GHATI60_DESC" VALUES (3,3,'Naga','3rd GHATI NAGA','Lord of snakes with mystic powers','en');
INSERT INTO "GHATI60_DESC" VALUES (4,4,'Kubera','4th GHATI KUBERA','Lord of treasure and wealth','en');
INSERT INTO "GHATI60_DESC" VALUES (5,5,'Yaksha','5th GHATI YAKSHA','Lord of mystic abilities','en');
INSERT INTO "GHATI60_DESC" VALUES (6,6,'Kinara','6th GHATI KINARA','Lord of heavenly music','en');
INSERT INTO "GHATI60_DESC" VALUES (7,7,'Bhrashtra','7th GHATI BHRASHTRA','Lord of falls, loss of path and error','en');
INSERT INTO "GHATI60_DESC" VALUES (8,8,'Kulagna','8th GHATI KULAGNA','Lord of family fire','en');
INSERT INTO "GHATI60_DESC" VALUES (9,9,'Visha','9th GHATI VISHA','Lord of the Poisons','en');
INSERT INTO "GHATI60_DESC" VALUES (10,10,'Agni','10th GHATI AGNI','Lord of fire','en');
INSERT INTO "GHATI60_DESC" VALUES (11,11,'Maya','11th GHATI MAYA','Lord of illusion and deception','en');
INSERT INTO "GHATI60_DESC" VALUES (12,12,'Pretapurisha','12th GHATI PRETAPURISHA','Lord of the land of ghosts who have died','en');
INSERT INTO "GHATI60_DESC" VALUES (13,13,'Apampathi','13th GHATI APAMPATHI','Lord of the waters','en');
INSERT INTO "GHATI60_DESC" VALUES (14,14,'Marutvana','14th GHATI MARUTVANA','Lord of the wind and thunderstorms','en');
INSERT INTO "GHATI60_DESC" VALUES (15,15,'Kala','15th GHATI KALA','Sovereign of time','en');
INSERT INTO "GHATI60_DESC" VALUES (16,16,'Shesha','16th GHATI SHESHA','Lord of constancy and eternity','en');
INSERT INTO "GHATI60_DESC" VALUES (17,17,'Avamrita','17th GHATI AVAMRITA','Lord of the Nectar of Longevity','en');
INSERT INTO "GHATI60_DESC" VALUES (18,18,'Soma','18th GHATI SOMA','Lord of the drink of pleasure','en');
INSERT INTO "GHATI60_DESC" VALUES (19,19,'Mridva','19th GHATI MRIDVA','Lord of affection, gentleness','en');
INSERT INTO "GHATI60_DESC" VALUES (20,20,'Komala','20th GHATI KOMALA','Lord of charm, acceptance','en');
INSERT INTO "GHATI60_DESC" VALUES (21,21,'Padma','21st GHATI PADMA','Lord of the Lotus','en');
INSERT INTO "GHATI60_DESC" VALUES (22,22,'Brahma','22st GHATI BRAHMA','Lord of the beginnings','en');
INSERT INTO "GHATI60_DESC" VALUES (23,23,'Dishta','23st GHATI DISHTA','Lord of the inevitable','en');
INSERT INTO "GHATI60_DESC" VALUES (24,24,'Digambara','24th GHATI DIGAMBARA','Lord of the inevitable','en');
INSERT INTO "GHATI60_DESC" VALUES (25,25,'Deva','25th GHATI DEVA','Lord of heaven','en');
INSERT INTO "GHATI60_DESC" VALUES (26,26,'Ardra','26th GHATI ARDRA','Lord of Tears','en');
INSERT INTO "GHATI60_DESC" VALUES (27,27,'Kalinasha','27th GHATI KALINASHA','Lord of the Lost Time','en');
INSERT INTO "GHATI60_DESC" VALUES (28,28,'Kshitishwara','28th GHATI KSHITISHWARA','Lord of the earth','en');
INSERT INTO "GHATI60_DESC" VALUES (29,29,'Kamalakara','29th GHATI KAMALAKARA','Lord of the lotuses','en');
INSERT INTO "GHATI60_DESC" VALUES (30,30,'Mandamaja','30th GHATI MANDAMAJA','Lord of the slow beginning, slow birth','en');
INSERT INTO "GHATI60_DESC" VALUES (31,31,'Mrityu','31st GHATI MRITYU','Lord of death','en');
INSERT INTO "GHATI60_DESC" VALUES (32,32,'Asita','32nd GHATI ASITA','Lord of the Dark','en');
INSERT INTO "GHATI60_DESC" VALUES (33,33,'Davagni','33rd GHATI DAVAGNI','Lord of the forest fire','en');
INSERT INTO "GHATI60_DESC" VALUES (34,34,'Samshaya','34th GHATI SAMSHAYA','Lord of doubt','en');
INSERT INTO "GHATI60_DESC" VALUES (35,35,'Yama','35th GHATI YAMA','Lord of transport, twins, severity, law, death','en');
INSERT INTO "GHATI60_DESC" VALUES (36,36,'Kantaka','36th GHATI KANTAKA','Lord of obstacles, spikes and pain','en');
INSERT INTO "GHATI60_DESC" VALUES (37,37,'Sudha','37th GHATI SUDHA','Lord of nectar, honey, prosperity and comfort','en');
INSERT INTO "GHATI60_DESC" VALUES (38,38,'Amrita','38th GHATI AMRITA','Lord of the drink of longevity, immortality','en');
INSERT INTO "GHATI60_DESC" VALUES (39,39,'Purnachandra','39th GHATI PURNACHANDRA','Lord of the night light','en');
INSERT INTO "GHATI60_DESC" VALUES (40,40,'Vishapradagdha','40th GHATI VISHAPRADAGDHA','Lord of antidotes','en');
INSERT INTO "GHATI60_DESC" VALUES (41,41,'Kulanasha','41st GHATI KULANASHA','Lord of family loss','en');
INSERT INTO "GHATI60_DESC" VALUES (42,42,'Vamshaksya','42nd GHATI VAMSHAKSYA','Lord of renunciation of pernicious qualities','en');
INSERT INTO "GHATI60_DESC" VALUES (43,43,'Utpataka','43rd GHATI UTPATAKA','Sovereign of flight, rise up, elevation','en');
INSERT INTO "GHATI60_DESC" VALUES (44,44,'Rupa','44th GHATI RUPA','Lord of forms and their transformations, changes','en');
INSERT INTO "GHATI60_DESC" VALUES (45,45,'Saumya','45th GHATI SAUMYA','Lord of appeasement and auspiciousness','en');
INSERT INTO "GHATI60_DESC" VALUES (46,46,'Mridu','46th GHATI MRIDU','Lord of reverence, courtesy, tenderness and gentleness','en');
INSERT INTO "GHATI60_DESC" VALUES (47,47,'Shitala','47th GHATI SHITALA','Lord of calm and cool','en');
INSERT INTO "GHATI60_DESC" VALUES (48,48,'Kadamshtra','48th GHATI KADAMSHTRA','Lord of Obstacles','en');
INSERT INTO "GHATI60_DESC" VALUES (49,49,'Indumukha','49th GHATI INDUMUKHA','Lord of Beauty','en');
INSERT INTO "GHATI60_DESC" VALUES (50,50,'Pravina','50th GHATI PRAVINA','Lord of Qualifications, Knowledge, Awareness','en');
INSERT INTO "GHATI60_DESC" VALUES (51,51,'Kalabala','51st GHATI KALABALA','Lord of the power of time','en');
INSERT INTO "GHATI60_DESC" VALUES (52,52,'Danda','52nd GHATI DANDA','Lord of the staff and reverence','en');
INSERT INTO "GHATI60_DESC" VALUES (53,53,'Nirmala','53rd GHATI NIRMALA','Lord of purity and chastity','en');
INSERT INTO "GHATI60_DESC" VALUES (54,54,'Shubha','54th GHATI SHUBHA','Lord of Virtue','en');
INSERT INTO "GHATI60_DESC" VALUES (55,55,'Ashubha','55th GHATI ASHUBHA','Lord of Trials','en');
INSERT INTO "GHATI60_DESC" VALUES (56,56,'Dala','56th GHATI DALA','Lord of detachment','en');
INSERT INTO "GHATI60_DESC" VALUES (57,57,'Amrita','57th GHATI AMRITA','Lord of the Nectar of Well-Being','en');
INSERT INTO "GHATI60_DESC" VALUES (58,58,'Payodhividya','58th GHATI PAYODHIVIDYA','Lord of the Ocean of Knowledge','en');
INSERT INTO "GHATI60_DESC" VALUES (59,59,'Bhrama','59th GHATI BHRAMA','Lord of wanderings','en');
INSERT INTO "GHATI60_DESC" VALUES (60,60,'Rekha','60th GHATI REKHA','Lord of lines and borders','en');
INSERT INTO "GHATI60_DESC" VALUES (61,1,'Гхора','1-й ГХАТИ ГХОРА','Владыка ужаса, уважения, потрясений, магических влияний','ru');
INSERT INTO "GHATI60_DESC" VALUES (62,2,'Ракшас','2-й ГХАТИ РАКШАС','Владыка стражей, охранения, причинения вреда, разрушений и испытаний','ru');
INSERT INTO "GHATI60_DESC" VALUES (63,3,'Нага','3-й ГХАТИ НАГА','Владыка змей, обладающих мистическими способностями','ru');
INSERT INTO "GHATI60_DESC" VALUES (64,4,'Кубера','4-й ГХАТИ КУБЕРА','Владыка сокровищ и богатства','ru');
INSERT INTO "GHATI60_DESC" VALUES (65,5,'Йакша','5-й ГХАТИ ЙАКША','Владыка мистических способностей','ru');
INSERT INTO "GHATI60_DESC" VALUES (66,6,'Кинара','6-й ГХАТИ КИНАРА','Владыка небесной музыки','ru');
INSERT INTO "GHATI60_DESC" VALUES (67,7,'Бхраштра','7-й ГХАТИ БХРАШТРА','Владыка падений, потери пути и ошибок','ru');
INSERT INTO "GHATI60_DESC" VALUES (68,8,'Кулагна','8-й ГХАТИ КУЛАГНА','Владыка семейного огня','ru');
INSERT INTO "GHATI60_DESC" VALUES (69,9,'Виша','9-й ГХАТИ ВИША','Владыка ядов','ru');
INSERT INTO "GHATI60_DESC" VALUES (70,10,'Агни','10-й ГХАТИ АГНИ','Владыка огня','ru');
INSERT INTO "GHATI60_DESC" VALUES (71,11,'Майа','11-й ГХАТИ МАЙА','Владыка иллюзии и обмана','ru');
INSERT INTO "GHATI60_DESC" VALUES (72,12,'Претапуриша','12-й ГХАТИ ПРЕТАПУРИША','Владыка земли призраков, умерших','ru');
INSERT INTO "GHATI60_DESC" VALUES (73,13,'Апампати','13-й ГХАТИ АПАМПАТИ','Владыка вод','ru');
INSERT INTO "GHATI60_DESC" VALUES (74,14,'Марутвана','14-й ГХАТИ МАРУТВАНА','Владыка ветра и гроз','ru');
INSERT INTO "GHATI60_DESC" VALUES (75,15,'Кала','15-й ГХАТИ КАЛА','Владыка времени','ru');
INSERT INTO "GHATI60_DESC" VALUES (76,16,'Шеша','16-й ГХАТИ ШЕША','Владыка постоянства и вечности','ru');
INSERT INTO "GHATI60_DESC" VALUES (77,17,'Авамрита','17-й ГХАТИ АВАМРИТА','Владыка нектара долголетия','ru');
INSERT INTO "GHATI60_DESC" VALUES (78,18,'Сома','18-й ГХАТИ СОМА','Владыка напитка наслаждения','ru');
INSERT INTO "GHATI60_DESC" VALUES (79,19,'Мридва','19-й ГХАТИ МРИДВА','Владыка ласки, мягкости','ru');
INSERT INTO "GHATI60_DESC" VALUES (80,20,'Комала','20-й ГХАТИ КОМАЛА','Владыка очарования, приятия','ru');
INSERT INTO "GHATI60_DESC" VALUES (81,21,'Падма','21-й ГХАТИ ПАДМА','Владыка Лотосов','ru');
INSERT INTO "GHATI60_DESC" VALUES (82,22,'Брахма','22-й ГХАТИ БРАХМА','Владыка начинаний','ru');
INSERT INTO "GHATI60_DESC" VALUES (83,23,'Дишта','23-й ГХАТИ ДИШТА','Владыка неизбежного','ru');
INSERT INTO "GHATI60_DESC" VALUES (84,24,'Дигамбара','24-й ГХАТИ ДИГАМБАРА','Владыка нужды','ru');
INSERT INTO "GHATI60_DESC" VALUES (85,25,'Дева','25-й ГХАТИ ДЕВА','Владыка небесного','ru');
INSERT INTO "GHATI60_DESC" VALUES (86,26,'Ардра','26-й ГХАТИ АРДРА','Владыка слез','ru');
INSERT INTO "GHATI60_DESC" VALUES (87,27,'Калинаша','27-й ГХАТИ КАЛИНАША','Владыка утраченного времени','ru');
INSERT INTO "GHATI60_DESC" VALUES (88,28,'Кшитишвара','28-й ГХАТИ КШИТИШВАРА','Владыка земли','ru');
INSERT INTO "GHATI60_DESC" VALUES (89,29,'Камалакара','29-й ГХАТИ КАМАЛАКАРА','Владыка лотосов','ru');
INSERT INTO "GHATI60_DESC" VALUES (90,30,'Мандамаджа','30-й ГХАТИ МАНДАМАДЖА','Владыка медленного начала, медленного рождения','ru');
INSERT INTO "GHATI60_DESC" VALUES (91,31,'Мритйу','31-й ГХАТИ МРИТЙУ','Владыка смерти','ru');
INSERT INTO "GHATI60_DESC" VALUES (92,32,'Асита','32-й ГХАТИ АСИТА','Владыка темноты','ru');
INSERT INTO "GHATI60_DESC" VALUES (93,33,'Давагни','33-й ГХАТИ ДАВАГНИ','Владыка лесного огня','ru');
INSERT INTO "GHATI60_DESC" VALUES (94,34,'Самшайа','34-й ГХАТИ САМШАЙА','Владыка сомнений','ru');
INSERT INTO "GHATI60_DESC" VALUES (95,35,'Йама','35-й ГХАТИ ЙАМА','Владыка транспорта, близнецов, строгости, закона, смерти','ru');
INSERT INTO "GHATI60_DESC" VALUES (96,36,'Кантака','36-й ГХАТИ КАНТАКА','Владыка препятствий, шипов и боли','ru');
INSERT INTO "GHATI60_DESC" VALUES (97,37,'Судха','37-й ГХАТИ СУДХА','Владыка нектара, меда, благоденствия и комфорта','ru');
INSERT INTO "GHATI60_DESC" VALUES (98,38,'Амрита','38-й ГХАТИ АМРИТА','Владыка напитка долголетия, бессмертия','ru');
INSERT INTO "GHATI60_DESC" VALUES (99,39,'Пурначандра','39-й ГХАТИ ПУРНАЧАНДРА','Владыка ночного света','ru');
INSERT INTO "GHATI60_DESC" VALUES (100,40,'Вишапрадагдха','40-й ГХАТИ ВИШАПРАДАГДХА','Владыка противоядий','ru');
INSERT INTO "GHATI60_DESC" VALUES (101,41,'Куланаша','41-й ГХАТИ КУЛАНАША','Владыка потери семьи','ru');
INSERT INTO "GHATI60_DESC" VALUES (102,42,'Вамшакшйа','42-й ГХАТИ ВАМШАКШЙА','Владыка отречения от пагубных качеств','ru');
INSERT INTO "GHATI60_DESC" VALUES (103,43,'Утпатака','43-й ГХАТИ УТПАТАКА','Владыка полетов, подъема вверх, возвышения','ru');
INSERT INTO "GHATI60_DESC" VALUES (104,44,'Рупа','44-й ГХАТИ РУПА','Владыка форм и их преображений, изменений','ru');
INSERT INTO "GHATI60_DESC" VALUES (105,45,'Саумйа','45-й ГХАТИ САУМЙА','Владыка умиротворения и благоприятности','ru');
INSERT INTO "GHATI60_DESC" VALUES (106,46,'Мриду','46-й ГХАТИ МРИДУ','Владыка почтения, учтивости, нежности и мягкости','ru');
INSERT INTO "GHATI60_DESC" VALUES (107,47,'Шитала','47-й ГХАТИ ШИТАЛА','Владыка спокойствия и прохлады','ru');
INSERT INTO "GHATI60_DESC" VALUES (108,48,'Кадамштра','48-й ГХАТИ КАДАМШТРА','Владыка устранения препятствий','ru');
INSERT INTO "GHATI60_DESC" VALUES (109,49,'Индумукха','49-й ГХАТИ ИНДУМУКХА','Владыка красоты','ru');
INSERT INTO "GHATI60_DESC" VALUES (110,50,'Правина','50-й ГХАТИ ПРАВИНА','Владыка квалифицированности, знаний, осведомленности','ru');
INSERT INTO "GHATI60_DESC" VALUES (111,51,'Калабала','51-й ГХАТИ КАЛАБАЛА','Владыка силы времени','ru');
INSERT INTO "GHATI60_DESC" VALUES (112,52,'Данда','52-й ГХАТИ ДАНДА','Владыка посоха и поклонов','ru');
INSERT INTO "GHATI60_DESC" VALUES (113,53,'Нирмала','53-й ГХАТИ НИРМАЛА','Владыка чистоты и целомудрия','ru');
INSERT INTO "GHATI60_DESC" VALUES (114,54,'Шубха','54-й ГХАТИ ШУБХА','Владыка добродетели','ru');
INSERT INTO "GHATI60_DESC" VALUES (115,55,'Ашубха','55-й ГХАТИ АШУБХА','Владыка испытаний','ru');
INSERT INTO "GHATI60_DESC" VALUES (116,56,'Дала','56-й ГХАТИ ДАЛА','Владыка отрешенности','ru');
INSERT INTO "GHATI60_DESC" VALUES (117,57,'Амрита','57-й ГХАТИ АМРИТА','Владыка нектара благополучия','ru');
INSERT INTO "GHATI60_DESC" VALUES (118,58,'Пайодхивидйа','58-й ГХАТИ ПАЙОДХИВИДЙА','Владыка океана знаний','ru');
INSERT INTO "GHATI60_DESC" VALUES (119,59,'Бхрама','59-й ГХАТИ БХРАМА','Владыка странствий, скитаний','ru');
INSERT INTO "GHATI60_DESC" VALUES (120,60,'Рекха','60-й ГХАТИ РЕКХА','Владыка линий и границ','ru');
INSERT INTO "KARANA" VALUES (1,1,1,5);
INSERT INTO "KARANA" VALUES (2,1,2,1);
INSERT INTO "KARANA" VALUES (3,2,1,1);
INSERT INTO "KARANA" VALUES (4,2,2,1);
INSERT INTO "KARANA" VALUES (5,3,1,1);
INSERT INTO "KARANA" VALUES (6,3,2,1);
INSERT INTO "KARANA" VALUES (7,4,1,1);
INSERT INTO "KARANA" VALUES (8,4,2,2);
INSERT INTO "KARANA" VALUES (9,5,1,1);
INSERT INTO "KARANA" VALUES (10,5,2,1);
INSERT INTO "KARANA" VALUES (11,6,1,1);
INSERT INTO "KARANA" VALUES (12,6,2,1);
INSERT INTO "KARANA" VALUES (13,7,1,1);
INSERT INTO "KARANA" VALUES (14,7,2,1);
INSERT INTO "KARANA" VALUES (15,8,1,2);
INSERT INTO "KARANA" VALUES (16,8,2,1);
INSERT INTO "KARANA" VALUES (17,9,1,1);
INSERT INTO "KARANA" VALUES (18,9,2,1);
INSERT INTO "KARANA" VALUES (19,10,1,1);
INSERT INTO "KARANA" VALUES (20,10,2,1);
INSERT INTO "KARANA" VALUES (21,11,1,1);
INSERT INTO "KARANA" VALUES (22,11,2,2);
INSERT INTO "KARANA" VALUES (23,12,1,1);
INSERT INTO "KARANA" VALUES (24,12,2,1);
INSERT INTO "KARANA" VALUES (25,13,1,1);
INSERT INTO "KARANA" VALUES (26,13,2,1);
INSERT INTO "KARANA" VALUES (27,14,1,1);
INSERT INTO "KARANA" VALUES (28,14,2,1);
INSERT INTO "KARANA" VALUES (29,15,1,2);
INSERT INTO "KARANA" VALUES (30,15,2,1);
INSERT INTO "KARANA" VALUES (31,16,1,1);
INSERT INTO "KARANA" VALUES (32,16,2,1);
INSERT INTO "KARANA" VALUES (33,17,1,1);
INSERT INTO "KARANA" VALUES (34,17,2,1);
INSERT INTO "KARANA" VALUES (35,18,1,1);
INSERT INTO "KARANA" VALUES (36,18,2,2);
INSERT INTO "KARANA" VALUES (37,19,1,1);
INSERT INTO "KARANA" VALUES (38,19,2,1);
INSERT INTO "KARANA" VALUES (39,20,1,1);
INSERT INTO "KARANA" VALUES (40,20,2,1);
INSERT INTO "KARANA" VALUES (41,21,1,1);
INSERT INTO "KARANA" VALUES (42,21,2,1);
INSERT INTO "KARANA" VALUES (43,22,1,2);
INSERT INTO "KARANA" VALUES (44,22,2,1);
INSERT INTO "KARANA" VALUES (45,23,1,1);
INSERT INTO "KARANA" VALUES (46,23,2,1);
INSERT INTO "KARANA" VALUES (47,24,1,1);
INSERT INTO "KARANA" VALUES (48,24,2,1);
INSERT INTO "KARANA" VALUES (49,25,1,1);
INSERT INTO "KARANA" VALUES (50,25,2,2);
INSERT INTO "KARANA" VALUES (51,26,1,1);
INSERT INTO "KARANA" VALUES (52,26,2,1);
INSERT INTO "KARANA" VALUES (53,27,1,1);
INSERT INTO "KARANA" VALUES (54,27,2,1);
INSERT INTO "KARANA" VALUES (55,28,1,1);
INSERT INTO "KARANA" VALUES (56,28,2,1);
INSERT INTO "KARANA" VALUES (57,29,1,2);
INSERT INTO "KARANA" VALUES (58,29,2,5);
INSERT INTO "KARANA" VALUES (59,30,1,5);
INSERT INTO "KARANA" VALUES (60,30,2,5);
INSERT INTO "KARANA_DESC" VALUES (1,1,'Kimstughna','Maruta, Ketu, 10 house owner','Actions aimed at improving health; religious activity.','An unfavorable day for important things.','en');
INSERT INTO "KARANA_DESC" VALUES (2,2,'Bava','Indra, Sun','Actions to improve health; expansion, development of something; performing religious rites, ceremonies and spiritual practices.','','en');
INSERT INTO "KARANA_DESC" VALUES (3,3,'Balava','Brahma, Moon','Charity; performing religious rites, ceremonies; training.','','en');
INSERT INTO "KARANA_DESC" VALUES (4,4,'Kaulava','Mitra, Mars','Acquaintances, acquiring friends, dates, agreements (contracts, agreements).','','en');
INSERT INTO "KARANA_DESC" VALUES (5,5,'Taitila','Vishvakarma, Mercury','Performing actions associated with a large number of people, actions leading to fame, popularity; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (6,6,'Gara','Bhoomi, Jupiter','Agriculture, land cultivation, sowing; home economics; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (7,7,'Vanija','Lakshmi, Venera','Trade; enjoyment of comfort and luxury; meeting, creating relationships.','','en');
INSERT INTO "KARANA_DESC" VALUES (8,8,'Vishti','Yama, Saturn','Perform actions aimed at the destruction of enemies, obstacles.','Do not perform favorable actions. Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (9,9,'Bava','Indra, Sun','Actions to improve health; expansion, development of something; performing religious rites, ceremonies and spiritual practices.','','en');
INSERT INTO "KARANA_DESC" VALUES (10,10,'Balava','Brahma, Moon','Charity; performing religious rites, ceremonies; training.','','en');
INSERT INTO "KARANA_DESC" VALUES (11,11,'Kaulava','Mitra, Mars','Acquaintances, acquiring friends, dates, agreements (contracts, agreements).','','en');
INSERT INTO "KARANA_DESC" VALUES (12,12,'Taitila','Vishvakarma, Mercury','Performing actions associated with a large number of people, actions leading to fame, popularity; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (13,13,'Gara','Bhoomi, Jupiter','Agriculture, land cultivation, sowing; home economics; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (14,14,'Vanija','Lakshmi, Venera','Trade; enjoyment of comfort and luxury; meeting, creating relationships.','','en');
INSERT INTO "KARANA_DESC" VALUES (15,15,'Vishti','Yama, Saturn','Perform actions aimed at the destruction of enemies, obstacles.','Do not perform favorable actions. Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (16,16,'Bava','Indra, Sun','Actions to improve health; expansion, development of something; performing religious rites, ceremonies and spiritual practices.','','en');
INSERT INTO "KARANA_DESC" VALUES (17,17,'Balava','Brahma, Moon','Charity; performing religious rites, ceremonies; training.','','en');
INSERT INTO "KARANA_DESC" VALUES (18,18,'Kaulava','Mitra, Mars','Acquaintances, acquiring friends, dates, agreements (contracts, agreements).','','en');
INSERT INTO "KARANA_DESC" VALUES (19,19,'Taitila','Vishvakarma, Mercury','Performing actions associated with a large number of people, actions leading to fame, popularity; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (20,20,'Gara','Bhoomi, Jupiter','Agriculture, land cultivation, sowing; home economics; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (21,21,'Vanija','Lakshmi, Venera','Trade; enjoyment of comfort and luxury; meeting, creating relationships.','','en');
INSERT INTO "KARANA_DESC" VALUES (22,22,'Vishti','Yama, Saturn','Perform actions aimed at the destruction of enemies, obstacles.','Do not perform favorable actions. Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (23,23,'Bava','Indra, Sun','Actions to improve health; expansion, development of something; performing religious rites, ceremonies and spiritual practices.','','en');
INSERT INTO "KARANA_DESC" VALUES (24,24,'Balava','Brahma, Moon','Charity; performing religious rites, ceremonies; training.','','en');
INSERT INTO "KARANA_DESC" VALUES (25,25,'Kaulava','Mitra, Mars','Acquaintances, acquiring friends, dates, agreements (contracts, agreements).','','en');
INSERT INTO "KARANA_DESC" VALUES (26,26,'Taitila','Vishvakarma, Mercury','Performing actions associated with a large number of people, actions leading to fame, popularity; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (27,27,'Gara','Bhoomi, Jupiter','Agriculture, land cultivation, sowing; home economics; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (28,28,'Vanija','Lakshmi, Venera','Trade; enjoyment of comfort and luxury; meeting, creating relationships.','','en');
INSERT INTO "KARANA_DESC" VALUES (29,29,'Vishti','Yama, Saturn','Perform actions aimed at the destruction of enemies, obstacles.','Do not perform favorable actions. Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (30,30,'Bava','Indra, Sun','Actions to improve health; expansion, development of something; performing religious rites, ceremonies and spiritual practices.','','en');
INSERT INTO "KARANA_DESC" VALUES (31,31,'Balava','Brahma, Moon','Charity; performing religious rites, ceremonies; training.','','en');
INSERT INTO "KARANA_DESC" VALUES (32,32,'Kaulava','Mitra, Mars','Acquaintances, acquiring friends, dates, agreements (contracts, agreements).','','en');
INSERT INTO "KARANA_DESC" VALUES (33,33,'Taitila','Vishvakarma, Mercury','Performing actions associated with a large number of people, actions leading to fame, popularity; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (34,34,'Gara','Bhoomi, Jupiter','Agriculture, land cultivation, sowing; home economics; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (35,35,'Vanija','Lakshmi, Venera','Trade; enjoyment of comfort and luxury; meeting, creating relationships.','','en');
INSERT INTO "KARANA_DESC" VALUES (36,36,'Vishti','Yama, Saturn','Perform actions aimed at the destruction of enemies, obstacles.','Do not perform favorable actions. Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (37,37,'Bava','Indra, Sun','Actions to improve health; expansion, development of something; performing religious rites, ceremonies and spiritual practices.','','en');
INSERT INTO "KARANA_DESC" VALUES (38,38,'Balava','Brahma, Moon','Charity; performing religious rites, ceremonies; training.','','en');
INSERT INTO "KARANA_DESC" VALUES (39,39,'Kaulava','Mitra, Mars','Acquaintances, acquiring friends, dates, agreements (contracts, agreements).','','en');
INSERT INTO "KARANA_DESC" VALUES (40,40,'Taitila','Vishvakarma, Mercury','Performing actions associated with a large number of people, actions leading to fame, popularity; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (41,41,'Gara','Bhoomi, Jupiter','Agriculture, land cultivation, sowing; home economics; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (42,42,'Vanija','Lakshmi, Venera','Trade; enjoyment of comfort and luxury; meeting, creating relationships.','','en');
INSERT INTO "KARANA_DESC" VALUES (43,43,'Vishti','Yama, Saturn','Perform actions aimed at the destruction of enemies, obstacles.','Do not perform favorable actions. Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (44,44,'Bava','Indra, Sun','Actions to improve health; expansion, development of something; performing religious rites, ceremonies and spiritual practices.','','en');
INSERT INTO "KARANA_DESC" VALUES (45,45,'Balava','Brahma, Moon','Charity; performing religious rites, ceremonies; training.','','en');
INSERT INTO "KARANA_DESC" VALUES (46,46,'Kaulava','Mitra, Mars','Acquaintances, acquiring friends, dates, agreements (contracts, agreements).','','en');
INSERT INTO "KARANA_DESC" VALUES (47,47,'Taitila','Vishvakarma, Mercury','Performing actions associated with a large number of people, actions leading to fame, popularity; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (48,48,'Gara','Bhoomi, Jupiter','Agriculture, land cultivation, sowing; home economics; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (49,49,'Vanija','Lakshmi, Venera','Trade; enjoyment of comfort and luxury; meeting, creating relationships.','','en');
INSERT INTO "KARANA_DESC" VALUES (50,50,'Vishti','Yama, Saturn','Perform actions aimed at the destruction of enemies, obstacles.','Do not perform favorable actions. Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (51,51,'Bava','Indra, Sun','Actions to improve health; expansion, development of something; performing religious rites, ceremonies and spiritual practices.','','en');
INSERT INTO "KARANA_DESC" VALUES (52,52,'Balava','Brahma, Moon','Charity; performing religious rites, ceremonies; training.','','en');
INSERT INTO "KARANA_DESC" VALUES (53,53,'Kaulava','Mitra, Mars','Acquaintances, acquiring friends, dates, agreements (contracts, agreements).','','en');
INSERT INTO "KARANA_DESC" VALUES (54,54,'Taitila','Vishvakarma, Mercury','Performing actions associated with a large number of people, actions leading to fame, popularity; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (55,55,'Gara','Bhoomi, Jupiter','Agriculture, land cultivation, sowing; home economics; building.','','en');
INSERT INTO "KARANA_DESC" VALUES (56,56,'Vanija','Lakshmi, Venera','Trade; enjoyment of comfort and luxury; meeting, creating relationships.','','en');
INSERT INTO "KARANA_DESC" VALUES (57,57,'Vishti','Yama, Saturn','Perform actions aimed at the destruction of enemies, obstacles.','Do not perform favorable actions. Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (58,58,'Shakuni','Kali, Rahu, Lagnesha','Taking action to improve health; taking medications; reading mantras.','Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (59,59,'Chatushpada','Rudra, Ketu, house 4 owner','Politics, government affairs; actions regarding ancestors; performing religious rites; animal husbandry.','Unfavorable day for important matters.','en');
INSERT INTO "KARANA_DESC" VALUES (60,60,'Naga','Nagi, Rahu, house 7 owner','Suitable for cruel cases, tricks, evil actions, performing work with fixed objects.','Unfavorable day for important matters. Carries the impulse leading to failure, hostility.','en');
INSERT INTO "KARANA_DESC" VALUES (61,1,'Кинстугхна','Марута, Кету, хоз. 10 дома','Действия, направленные на улучшение здоровья; религиозная деятельность.','Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (62,2,'Бава','Индра, Солнце','Действия, направленные на улучшение здоровья; развитие, расширение, разработка чего-либо; выполнение религиозных обрядов, церемоний и духовных практик.','','ru');
INSERT INTO "KARANA_DESC" VALUES (63,3,'Балава','Брахма, Луна','Благотворительность; выполнение религиозных обрядов, церемоний; обучение.','','ru');
INSERT INTO "KARANA_DESC" VALUES (64,4,'Каулава','Митра, Марс','Знакомства, приобретение друзей, свидания, соглашения (контракты, договоры).','','ru');
INSERT INTO "KARANA_DESC" VALUES (65,5,'Таитила','Вишвакарма, Меркурий','Выполнение действий, связанных с большим количеством народа, действий ведущих к известности, популярности; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (66,6,'Гара','Бхуми, Юпитер','Занятия сельским хозяйством, возделывание земель, посевные работы; домоводство; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (67,7,'Ваниджа','Лакшми, Венера','Торговля; наслаждение комфортом и роскошью; встречи, создание отношений.','','ru');
INSERT INTO "KARANA_DESC" VALUES (68,8,'Вишти','Яма, Сатурн','Выполнять действия, направленные на разрушение врагов, препятствий.','Не выполнять благоприятных действий. Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (69,9,'Бава','Индра, Солнце','Действия, направленные на улучшение здоровья; развитие, расширение, разработка чего-либо; выполнение религиозных обрядов, церемоний и духовных практик.','','ru');
INSERT INTO "KARANA_DESC" VALUES (70,10,'Балава','Брахма, Луна','Благотворительность; выполнение религиозных обрядов, церемоний; обучение.','','ru');
INSERT INTO "KARANA_DESC" VALUES (71,11,'Каулава','Митра, Марс','Знакомства, приобретение друзей, свидания, соглашения (контракты, договоры).','','ru');
INSERT INTO "KARANA_DESC" VALUES (72,12,'Таитила','Вишвакарма, Меркурий','Выполнение действий, связанных с большим количеством народа, действий ведущих к известности, популярности; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (73,13,'Гара','Бхуми, Юпитер','Занятия сельским хозяйством, возделывание земель, посевные работы; домоводство; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (74,14,'Ваниджа','Лакшми, Венера','Торговля; наслаждение комфортом и роскошью; встречи, создание отношений.','','ru');
INSERT INTO "KARANA_DESC" VALUES (75,15,'Вишти','Яма, Сатурн','Выполнять действия, направленные на разрушение врагов, препятствий.','Не выполнять благоприятных действий. Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (76,16,'Бава','Индра, Солнце','Действия, направленные на улучшение здоровья; развитие, расширение, разработка чего-либо; выполнение религиозных обрядов, церемоний и духовных практик.','','ru');
INSERT INTO "KARANA_DESC" VALUES (77,17,'Балава','Брахма, Луна','Благотворительность; выполнение религиозных обрядов, церемоний; обучение.','','ru');
INSERT INTO "KARANA_DESC" VALUES (78,18,'Каулава','Митра, Марс','Знакомства, приобретение друзей, свидания, соглашения (контракты, договоры).','','ru');
INSERT INTO "KARANA_DESC" VALUES (79,19,'Таитила','Вишвакарма, Меркурий','Выполнение действий, связанных с большим количеством народа, действий ведущих к известности, популярности; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (80,20,'Гара','Бхуми, Юпитер','Занятия сельским хозяйством, возделывание земель, посевные работы; домоводство; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (81,21,'Ваниджа','Лакшми, Венера','Торговля; наслаждение комфортом и роскошью; встречи, создание отношений.','','ru');
INSERT INTO "KARANA_DESC" VALUES (82,22,'Вишти','Яма, Сатурн','Выполнять действия, направленные на разрушение врагов, препятствий.','Не выполнять благоприятных действий. Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (83,23,'Бава','Индра, Солнце','Действия, направленные на улучшение здоровья; развитие, расширение, разработка чего-либо; выполнение религиозных обрядов, церемоний и духовных практик.','','ru');
INSERT INTO "KARANA_DESC" VALUES (84,24,'Балава','Брахма, Луна','Благотворительность; выполнение религиозных обрядов, церемоний; обучение.','','ru');
INSERT INTO "KARANA_DESC" VALUES (85,25,'Каулава','Митра, Марс','Знакомства, приобретение друзей, свидания, соглашения (контракты, договоры).','','ru');
INSERT INTO "KARANA_DESC" VALUES (86,26,'Таитила','Вишвакарма, Меркурий','Выполнение действий, связанных с большим количеством народа, действий ведущих к известности, популярности; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (87,27,'Гара','Бхуми, Юпитер','Занятия сельским хозяйством, возделывание земель, посевные работы; домоводство; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (88,28,'Ваниджа','Лакшми, Венера','Торговля; наслаждение комфортом и роскошью; встречи, создание отношений.','','ru');
INSERT INTO "KARANA_DESC" VALUES (89,29,'Вишти','Яма, Сатурн','Выполнять действия, направленные на разрушение врагов, препятствий.','Не выполнять благоприятных действий. Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (90,30,'Бава','Индра, Солнце','Действия, направленные на улучшение здоровья; развитие, расширение, разработка чего-либо; выполнение религиозных обрядов, церемоний и духовных практик.','','ru');
INSERT INTO "KARANA_DESC" VALUES (91,31,'Балава','Брахма, Луна','Благотворительность; выполнение религиозных обрядов, церемоний; обучение.','','ru');
INSERT INTO "KARANA_DESC" VALUES (92,32,'Каулава','Митра, Марс','Знакомства, приобретение друзей, свидания, соглашения (контракты, договоры).','','ru');
INSERT INTO "KARANA_DESC" VALUES (93,33,'Таитила','Вишвакарма, Меркурий','Выполнение действий, связанных с большим количеством народа, действий ведущих к известности, популярности; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (94,34,'Гара','Бхуми, Юпитер','Занятия сельским хозяйством, возделывание земель, посевные работы; домоводство; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (95,35,'Ваниджа','Лакшми, Венера','Торговля; наслаждение комфортом и роскошью; встречи, создание отношений.','','ru');
INSERT INTO "KARANA_DESC" VALUES (96,36,'Вишти','Яма, Сатурн','Выполнять действия, направленные на разрушение врагов, препятствий.','Не выполнять благоприятных действий. Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (97,37,'Бава','Индра, Солнце','Действия, направленные на улучшение здоровья; развитие, расширение, разработка чего-либо; выполнение религиозных обрядов, церемоний и духовных практик.','','ru');
INSERT INTO "KARANA_DESC" VALUES (98,38,'Балава','Брахма, Луна','Благотворительность; выполнение религиозных обрядов, церемоний; обучение.','','ru');
INSERT INTO "KARANA_DESC" VALUES (99,39,'Каулава','Митра, Марс','Знакомства, приобретение друзей, свидания, соглашения (контракты, договоры).','','ru');
INSERT INTO "KARANA_DESC" VALUES (100,40,'Таитила','Вишвакарма, Меркурий','Выполнение действий, связанных с большим количеством народа, действий ведущих к известности, популярности; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (101,41,'Гара','Бхуми, Юпитер','Занятия сельским хозяйством, возделывание земель, посевные работы; домоводство; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (102,42,'Ваниджа','Лакшми, Венера','Торговля; наслаждение комфортом и роскошью; встречи, создание отношений.','','ru');
INSERT INTO "KARANA_DESC" VALUES (103,43,'Вишти','Яма, Сатурн','Выполнять действия, направленные на разрушение врагов, препятствий.','Не выполнять благоприятных действий. Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (104,44,'Бава','Индра, Солнце','Действия, направленные на улучшение здоровья; развитие, расширение, разработка чего-либо; выполнение религиозных обрядов, церемоний и духовных практик.','','ru');
INSERT INTO "KARANA_DESC" VALUES (105,45,'Балава','Брахма, Луна','Благотворительность; выполнение религиозных обрядов, церемоний; обучение.','','ru');
INSERT INTO "KARANA_DESC" VALUES (106,46,'Каулава','Митра, Марс','Знакомства, приобретение друзей, свидания, соглашения (контракты, договоры).','','ru');
INSERT INTO "KARANA_DESC" VALUES (107,47,'Таитила','Вишвакарма, Меркурий','Выполнение действий, связанных с большим количеством народа, действий ведущих к известности, популярности; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (108,48,'Гара','Бхуми, Юпитер','Занятия сельским хозяйством, возделывание земель, посевные работы; домоводство; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (109,49,'Ваниджа','Лакшми, Венера','Торговля; наслаждение комфортом и роскошью; встречи, создание отношений.','','ru');
INSERT INTO "KARANA_DESC" VALUES (110,50,'Вишти','Яма, Сатурн','Выполнять действия, направленные на разрушение врагов, препятствий.','Не выполнять благоприятных действий. Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (111,51,'Бава','Индра, Солнце','Действия, направленные на улучшение здоровья; развитие, расширение, разработка чего-либо; выполнение религиозных обрядов, церемоний и духовных практик.','','ru');
INSERT INTO "KARANA_DESC" VALUES (112,52,'Балава','Брахма, Луна','Благотворительность; выполнение религиозных обрядов, церемоний; обучение.','','ru');
INSERT INTO "KARANA_DESC" VALUES (113,53,'Каулава','Митра, Марс','Знакомства, приобретение друзей, свидания, соглашения (контракты, договоры).','','ru');
INSERT INTO "KARANA_DESC" VALUES (114,54,'Таитила','Вишвакарма, Меркурий','Выполнение действий, связанных с большим количеством народа, действий ведущих к известности, популярности; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (115,55,'Гара','Бхуми, Юпитер','Занятия сельским хозяйством, возделывание земель, посевные работы; домоводство; строительство.','','ru');
INSERT INTO "KARANA_DESC" VALUES (116,56,'Ваниджа','Лакшми, Венера','Торговля; наслаждение комфортом и роскошью; встречи, создание отношений.','','ru');
INSERT INTO "KARANA_DESC" VALUES (117,57,'Вишти','Яма, Сатурн','Выполнять действия, направленные на разрушение врагов, препятствий.','Не выполнять благоприятных действий. Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (118,58,'Шакуни','Кали, Раху, Лагнеша','Выполнение действий, направленных на улучшение здоровья; прием медицинских препаратов; чтение мантр.','Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (119,59,'Чатушпада','Рудра, Кету, хоз. 4 дома','Политика, государственные дела; действия касаемо предков; выполнение религиозных обрядов; животноводство.','Неблагоприятный день для важных дел.','ru');
INSERT INTO "KARANA_DESC" VALUES (120,60,'Нага','Наги, Раху, хоз. 7 дома','Подходит для жестоких дел, хитрости, злых действий, выполнение работ с неподвижными объектами.','Неблагоприятный день для важных дел. Несет в себе импульс, ведущий к неудачам, враждебности.','ru');
INSERT INTO "LANGUAGE" VALUES (1,'en','en-US');
INSERT INTO "LANGUAGE" VALUES (2,'uk','uk-UA');
INSERT INTO "LANGUAGE" VALUES (3,'pl','pl-PL');
INSERT INTO "LANGUAGE" VALUES (4,'ru','ru-RU');
INSERT INTO "LANGUAGE_DESC" VALUES (1,1,'English','en');
INSERT INTO "LANGUAGE_DESC" VALUES (2,2,'Ukrainian','en');
INSERT INTO "LANGUAGE_DESC" VALUES (3,3,'Polish','en');
INSERT INTO "LANGUAGE_DESC" VALUES (4,4,'Russian','en');
INSERT INTO "LANGUAGE_DESC" VALUES (5,1,'Английский','ru');
INSERT INTO "LANGUAGE_DESC" VALUES (6,2,'Украинский','ru');
INSERT INTO "LANGUAGE_DESC" VALUES (7,3,'Польский','ru');
INSERT INTO "LANGUAGE_DESC" VALUES (8,2,'Русский','ru');
INSERT INTO "LOCATION" VALUES (37460,'Київ','50.4501','30.5234','Київ','Київська область','Україна','UA','uk');
INSERT INTO "LOCATION" VALUES (37461,'Чорний Острів','49.3323','26.8484','Хмельницький район','Хмельницька область','Україна','UA','uk');
INSERT INTO "MASA" VALUES (1,12,'1,4','8,9,11,23,24,26');
INSERT INTO "MASA" VALUES (2,1,'14,15','12,27');
INSERT INTO "MASA" VALUES (3,2,'7,8,21','13,28,29');
INSERT INTO "MASA" VALUES (4,3,'11,23','6,7,21');
INSERT INTO "MASA" VALUES (5,4,'20,21,22','2,3,15,17,18,30');
INSERT INTO "MASA" VALUES (6,5,'24,27','1,2,7,16,17,22');
INSERT INTO "MASA" VALUES (7,6,'25','9,10,11,24,25,26');
INSERT INTO "MASA" VALUES (8,7,'3,5,8,10','5,14,20');
INSERT INTO "MASA" VALUES (9,8,'14,16,17,26','2,7,8,9,17,22,23,24');
INSERT INTO "MASA" VALUES (10,9,'1,6,9,13','1,4,5,16,19,20');
INSERT INTO "MASA" VALUES (11,10,'19,22','4,6,10,19,20,25');
INSERT INTO "MASA" VALUES (12,11,'2,18','3,14,19,29');
INSERT INTO "MASA_DESC" VALUES (1,1,'Chaitra','en');
INSERT INTO "MASA_DESC" VALUES (2,2,'Vaisakha','en');
INSERT INTO "MASA_DESC" VALUES (3,3,'Jyeshtha','en');
INSERT INTO "MASA_DESC" VALUES (4,4,'Ashadha','en');
INSERT INTO "MASA_DESC" VALUES (5,5,'Shravana','en');
INSERT INTO "MASA_DESC" VALUES (6,6,'Bhadrapada','en');
INSERT INTO "MASA_DESC" VALUES (7,7,'Ashvina','en');
INSERT INTO "MASA_DESC" VALUES (8,8,'Kartika','en');
INSERT INTO "MASA_DESC" VALUES (9,9,'Margasira','en');
INSERT INTO "MASA_DESC" VALUES (10,10,'Pushya','en');
INSERT INTO "MASA_DESC" VALUES (11,11,'Magha','en');
INSERT INTO "MASA_DESC" VALUES (12,12,'Phalguna','en');
INSERT INTO "MASA_DESC" VALUES (13,1,'Чаитра','ru');
INSERT INTO "MASA_DESC" VALUES (14,2,'Вайшакха','ru');
INSERT INTO "MASA_DESC" VALUES (15,3,'Джаештха','ru');
INSERT INTO "MASA_DESC" VALUES (16,4,'Ашадха','ru');
INSERT INTO "MASA_DESC" VALUES (17,5,'Шравана','ru');
INSERT INTO "MASA_DESC" VALUES (18,6,'Бхадрапада','ru');
INSERT INTO "MASA_DESC" VALUES (19,7,'Ашвина','ru');
INSERT INTO "MASA_DESC" VALUES (20,8,'Картика','ru');
INSERT INTO "MASA_DESC" VALUES (21,9,'Маргашира','ru');
INSERT INTO "MASA_DESC" VALUES (22,10,'Пушья','ru');
INSERT INTO "MASA_DESC" VALUES (23,11,'Магха','ru');
INSERT INTO "MASA_DESC" VALUES (24,12,'Пхалгуна','ru');
INSERT INTO "MRITYUBHAGA" VALUES (1,1,1,20);
INSERT INTO "MRITYUBHAGA" VALUES (2,1,2,39);
INSERT INTO "MRITYUBHAGA" VALUES (3,1,3,72);
INSERT INTO "MRITYUBHAGA" VALUES (4,1,4,96);
INSERT INTO "MRITYUBHAGA" VALUES (5,1,5,128);
INSERT INTO "MRITYUBHAGA" VALUES (6,1,6,174);
INSERT INTO "MRITYUBHAGA" VALUES (7,1,7,196);
INSERT INTO "MRITYUBHAGA" VALUES (8,1,8,227);
INSERT INTO "MRITYUBHAGA" VALUES (9,1,9,262);
INSERT INTO "MRITYUBHAGA" VALUES (10,1,10,272);
INSERT INTO "MRITYUBHAGA" VALUES (11,1,11,303);
INSERT INTO "MRITYUBHAGA" VALUES (12,1,12,353);
INSERT INTO "MRITYUBHAGA" VALUES (13,2,1,26);
INSERT INTO "MRITYUBHAGA" VALUES (14,2,2,42);
INSERT INTO "MRITYUBHAGA" VALUES (15,2,3,73);
INSERT INTO "MRITYUBHAGA" VALUES (16,2,4,115);
INSERT INTO "MRITYUBHAGA" VALUES (17,2,5,144);
INSERT INTO "MRITYUBHAGA" VALUES (18,2,6,161);
INSERT INTO "MRITYUBHAGA" VALUES (19,2,7,206);
INSERT INTO "MRITYUBHAGA" VALUES (20,2,8,224);
INSERT INTO "MRITYUBHAGA" VALUES (21,2,9,253);
INSERT INTO "MRITYUBHAGA" VALUES (22,2,10,295);
INSERT INTO "MRITYUBHAGA" VALUES (23,2,11,305);
INSERT INTO "MRITYUBHAGA" VALUES (24,2,12,342);
INSERT INTO "MRITYUBHAGA" VALUES (25,3,1,19);
INSERT INTO "MRITYUBHAGA" VALUES (26,3,2,58);
INSERT INTO "MRITYUBHAGA" VALUES (27,3,3,85);
INSERT INTO "MRITYUBHAGA" VALUES (28,3,4,113);
INSERT INTO "MRITYUBHAGA" VALUES (29,3,5,149);
INSERT INTO "MRITYUBHAGA" VALUES (30,3,6,178);
INSERT INTO "MRITYUBHAGA" VALUES (31,3,7,194);
INSERT INTO "MRITYUBHAGA" VALUES (32,3,8,231);
INSERT INTO "MRITYUBHAGA" VALUES (33,3,9,242);
INSERT INTO "MRITYUBHAGA" VALUES (34,3,10,285);
INSERT INTO "MRITYUBHAGA" VALUES (35,3,11,311);
INSERT INTO "MRITYUBHAGA" VALUES (36,3,12,336);
INSERT INTO "MRITYUBHAGA" VALUES (37,4,1,15);
INSERT INTO "MRITYUBHAGA" VALUES (38,4,2,44);
INSERT INTO "MRITYUBHAGA" VALUES (39,4,3,73);
INSERT INTO "MRITYUBHAGA" VALUES (40,4,4,102);
INSERT INTO "MRITYUBHAGA" VALUES (41,4,5,128);
INSERT INTO "MRITYUBHAGA" VALUES (42,4,6,168);
INSERT INTO "MRITYUBHAGA" VALUES (43,4,7,200);
INSERT INTO "MRITYUBHAGA" VALUES (44,4,8,220);
INSERT INTO "MRITYUBHAGA" VALUES (45,4,9,261);
INSERT INTO "MRITYUBHAGA" VALUES (46,4,10,292);
INSERT INTO "MRITYUBHAGA" VALUES (47,4,11,307);
INSERT INTO "MRITYUBHAGA" VALUES (48,4,12,335);
INSERT INTO "MRITYUBHAGA" VALUES (49,5,1,19);
INSERT INTO "MRITYUBHAGA" VALUES (50,5,2,59);
INSERT INTO "MRITYUBHAGA" VALUES (51,5,3,72);
INSERT INTO "MRITYUBHAGA" VALUES (52,5,4,117);
INSERT INTO "MRITYUBHAGA" VALUES (53,5,5,126);
INSERT INTO "MRITYUBHAGA" VALUES (54,5,6,154);
INSERT INTO "MRITYUBHAGA" VALUES (55,5,7,193);
INSERT INTO "MRITYUBHAGA" VALUES (56,5,8,220);
INSERT INTO "MRITYUBHAGA" VALUES (57,5,9,257);
INSERT INTO "MRITYUBHAGA" VALUES (58,5,10,281);
INSERT INTO "MRITYUBHAGA" VALUES (59,5,11,315);
INSERT INTO "MRITYUBHAGA" VALUES (60,5,12,358);
INSERT INTO "MRITYUBHAGA" VALUES (61,6,1,28);
INSERT INTO "MRITYUBHAGA" VALUES (62,6,2,45);
INSERT INTO "MRITYUBHAGA" VALUES (63,6,3,71);
INSERT INTO "MRITYUBHAGA" VALUES (64,6,4,107);
INSERT INTO "MRITYUBHAGA" VALUES (65,6,5,130);
INSERT INTO "MRITYUBHAGA" VALUES (66,6,6,163);
INSERT INTO "MRITYUBHAGA" VALUES (67,6,7,184);
INSERT INTO "MRITYUBHAGA" VALUES (68,6,8,216);
INSERT INTO "MRITYUBHAGA" VALUES (69,6,9,267);
INSERT INTO "MRITYUBHAGA" VALUES (70,6,10,282);
INSERT INTO "MRITYUBHAGA" VALUES (71,6,11,329);
INSERT INTO "MRITYUBHAGA" VALUES (72,6,12,349);
INSERT INTO "MRITYUBHAGA" VALUES (73,7,1,10);
INSERT INTO "MRITYUBHAGA" VALUES (74,7,2,34);
INSERT INTO "MRITYUBHAGA" VALUES (75,7,3,67);
INSERT INTO "MRITYUBHAGA" VALUES (76,7,4,99);
INSERT INTO "MRITYUBHAGA" VALUES (77,7,5,132);
INSERT INTO "MRITYUBHAGA" VALUES (78,7,6,166);
INSERT INTO "MRITYUBHAGA" VALUES (79,7,7,183);
INSERT INTO "MRITYUBHAGA" VALUES (80,7,8,228);
INSERT INTO "MRITYUBHAGA" VALUES (81,7,9,268);
INSERT INTO "MRITYUBHAGA" VALUES (82,7,10,284);
INSERT INTO "MRITYUBHAGA" VALUES (83,7,11,313);
INSERT INTO "MRITYUBHAGA" VALUES (84,7,12,345);
INSERT INTO "MRITYUBHAGA" VALUES (85,8,1,14);
INSERT INTO "MRITYUBHAGA" VALUES (86,8,2,43);
INSERT INTO "MRITYUBHAGA" VALUES (87,8,3,72);
INSERT INTO "MRITYUBHAGA" VALUES (88,8,4,101);
INSERT INTO "MRITYUBHAGA" VALUES (89,8,5,144);
INSERT INTO "MRITYUBHAGA" VALUES (90,8,6,173);
INSERT INTO "MRITYUBHAGA" VALUES (91,8,7,202);
INSERT INTO "MRITYUBHAGA" VALUES (92,8,8,231);
INSERT INTO "MRITYUBHAGA" VALUES (93,8,9,250);
INSERT INTO "MRITYUBHAGA" VALUES (94,8,10,290);
INSERT INTO "MRITYUBHAGA" VALUES (95,8,11,318);
INSERT INTO "MRITYUBHAGA" VALUES (96,8,12,338);
INSERT INTO "MRITYUBHAGA" VALUES (97,9,1,8);
INSERT INTO "MRITYUBHAGA" VALUES (98,9,2,48);
INSERT INTO "MRITYUBHAGA" VALUES (99,9,3,80);
INSERT INTO "MRITYUBHAGA" VALUES (100,9,4,100);
INSERT INTO "MRITYUBHAGA" VALUES (101,9,5,141);
INSERT INTO "MRITYUBHAGA" VALUES (102,9,6,172);
INSERT INTO "MRITYUBHAGA" VALUES (103,9,7,203);
INSERT INTO "MRITYUBHAGA" VALUES (104,9,8,234);
INSERT INTO "MRITYUBHAGA" VALUES (105,9,9,251);
INSERT INTO "MRITYUBHAGA" VALUES (106,9,10,282);
INSERT INTO "MRITYUBHAGA" VALUES (107,9,11,313);
INSERT INTO "MRITYUBHAGA" VALUES (108,9,12,344);
INSERT INTO "MUHURTA" VALUES (1,1,'ABHIJIT');
INSERT INTO "MUHURTA" VALUES (2,2,'RAHUKALA');
INSERT INTO "MUHURTA" VALUES (3,1,'BRAHMA');
INSERT INTO "MUHURTA" VALUES (4,2,'GULIKAKALA');
INSERT INTO "MUHURTA" VALUES (5,2,'YAMAGANDA');
INSERT INTO "MUHURTA30" VALUES (1,2,'RUDRA');
INSERT INTO "MUHURTA30" VALUES (2,2,'AKHI');
INSERT INTO "MUHURTA30" VALUES (3,1,'MITRA');
INSERT INTO "MUHURTA30" VALUES (4,2,'PITRI');
INSERT INTO "MUHURTA30" VALUES (5,1,'VASU');
INSERT INTO "MUHURTA30" VALUES (6,1,'VAYRA');
INSERT INTO "MUHURTA30" VALUES (7,1,'VISHVA');
INSERT INTO "MUHURTA30" VALUES (8,1,'ABHIDJIT');
INSERT INTO "MUHURTA30" VALUES (9,1,'RAUKHINA');
INSERT INTO "MUHURTA30" VALUES (10,2,'PURUHUTA');
INSERT INTO "MUHURTA30" VALUES (11,2,'WAHINI');
INSERT INTO "MUHURTA30" VALUES (12,2,'NIRRITI');
INSERT INTO "MUHURTA30" VALUES (13,1,'VARUNA');
INSERT INTO "MUHURTA30" VALUES (14,1,'ARYAMA');
INSERT INTO "MUHURTA30" VALUES (15,2,'BHAHA');
INSERT INTO "MUHURTA30" VALUES (16,2,'GIRISHA');
INSERT INTO "MUHURTA30" VALUES (17,2,'AJAPADA');
INSERT INTO "MUHURTA30" VALUES (18,1,'AKHIRBUDHNYA');
INSERT INTO "MUHURTA30" VALUES (19,1,'PUSHA');
INSERT INTO "MUHURTA30" VALUES (20,1,'ASHVI');
INSERT INTO "MUHURTA30" VALUES (21,2,'YAMA');
INSERT INTO "MUHURTA30" VALUES (22,2,'AGNI');
INSERT INTO "MUHURTA30" VALUES (23,1,'VIDHATRA');
INSERT INTO "MUHURTA30" VALUES (24,1,'CHANDRA');
INSERT INTO "MUHURTA30" VALUES (25,1,'ADITI');
INSERT INTO "MUHURTA30" VALUES (26,1,'JYVA');
INSERT INTO "MUHURTA30" VALUES (27,1,'VISHNU');
INSERT INTO "MUHURTA30" VALUES (28,1,'TAPAS');
INSERT INTO "MUHURTA30" VALUES (29,1,'BRAHMA');
INSERT INTO "MUHURTA30" VALUES (30,1,'VAYU');
INSERT INTO "MUHURTA30_DESC" VALUES (1,1,'Rudra','1st MUHURTA RAUDRA','Lord of screaming, roaring, fear, upheaval, destruction and change','en');
INSERT INTO "MUHURTA30_DESC" VALUES (2,2,'Akhi','2nd MUHURTA AKHI','Serpent lord','en');
INSERT INTO "MUHURTA30_DESC" VALUES (3,3,'Mitra','3rd MUHURTA MAITRA','Lord of friendliness','en');
INSERT INTO "MUHURTA30_DESC" VALUES (4,4,'Pitri','4th MUHURTA PITRA','Lord of the ancestors','en');
INSERT INTO "MUHURTA30_DESC" VALUES (5,5,'Vasu','5th MUHURTA SAVITRA (VASU)','Lord of the Life Force','en');
INSERT INTO "MUHURTA30_DESC" VALUES (6,6,'Vayra','6th MUHURTA VAYRA (AMBU)','Lord of enmity, revenge, quarrel, hostility','en');
INSERT INTO "MUHURTA30_DESC" VALUES (7,7,'Vishva','7th MUHURTA VISHVA (VISHVADEVA)','Lord of wholeness, fullness, unity, universality','en');
INSERT INTO "MUHURTA30_DESC" VALUES (8,8,'Abhidjit','8th MUHURTA ABHIDJIT (Vidhi)','Lord of victory and good luck','en');
INSERT INTO "MUHURTA30_DESC" VALUES (9,9,'Raukhina','9th MUHURTA RAUKHINA (Vidhata, Satamukhi)','Lord of the sandalwood','en');
INSERT INTO "MUHURTA30_DESC" VALUES (10,10,'Puruhuta','10th MUHURTA BALA (Puruhuta)','Lord of youth, youth, children','en');
INSERT INTO "MUHURTA30_DESC" VALUES (11,11,'Wahini','11th MUHURTA WAHI (Indraagni)','Lord of transportation','en');
INSERT INTO "MUHURTA30_DESC" VALUES (12,12,'Nirriti','12th MUHURTA NIRITTA (Naktanchara)','Lord of death','en');
INSERT INTO "MUHURTA30_DESC" VALUES (13,13,'Varuna','13th MUHURTA VARUNA','Lord of the water','en');
INSERT INTO "MUHURTA30_DESC" VALUES (14,14,'Aryama','14th MUHURTA ARYAMA','Lord of partnership, partnership, matchmaking','en');
INSERT INTO "MUHURTA30_DESC" VALUES (15,15,'Bhaha','15th MUHURTA BHAHA','Lord of prosperity, prosperity, good luck, family happiness','en');
INSERT INTO "MUHURTA30_DESC" VALUES (16,16,'Girisha','16th MUHURTA GIRISHA','Lord of the mountains','en');
INSERT INTO "MUHURTA30_DESC" VALUES (17,17,'Ajapada','17th MUHURTA AJAPADA','Lord of the first step','en');
INSERT INTO "MUHURTA30_DESC" VALUES (18,18,'Akhirbudhnya','18th MUHURTA AKHIRBUDHNYA','Lord of the Underworlds','en');
INSERT INTO "MUHURTA30_DESC" VALUES (19,19,'Pusha','19th MUHURTA PUSHA','Lord of growth, prosperity, herds and travel','en');
INSERT INTO "MUHURTA30_DESC" VALUES (20,20,'Ashvi','20th MUHURTA ASHVI','Lord of healing and movement','en');
INSERT INTO "MUHURTA30_DESC" VALUES (21,21,'Yama','21st MUHURTA YAMA','Lord of DHARMA and death','en');
INSERT INTO "MUHURTA30_DESC" VALUES (22,22,'Agni','22nd MUHURTA AGNI','Lord of fire','en');
INSERT INTO "MUHURTA30_DESC" VALUES (23,23,'Vidhatra','23rd MUHURTA VIDHATRA','Lord of Gifts and Distributions','en');
INSERT INTO "MUHURTA30_DESC" VALUES (24,24,'Chandra','24th MUHURTA RATRINATHA (Chandra)','Lord of the night','en');
INSERT INTO "MUHURTA30_DESC" VALUES (25,25,'Aditi','25th MUHURTA ADITYA','Lord of Blessings','en');
INSERT INTO "MUHURTA30_DESC" VALUES (26,26,'Jyva','26th MUHURTA JYVA','Sovereign of health and vitality','en');
INSERT INTO "MUHURTA30_DESC" VALUES (27,27,'Vishnu','27th MUHURTA VISHNU','Lord, Custodian and Patron','en');
INSERT INTO "MUHURTA30_DESC" VALUES (28,28,'Tapas','28th MUHURTA TAPAS (Yamigaduti)','Lord austerity','en');
INSERT INTO "MUHURTA30_DESC" VALUES (29,29,'Brahma','29th MUHURTA BRAHMA (Tyastur)','Master of creation','en');
INSERT INTO "MUHURTA30_DESC" VALUES (30,30,'Vayu','30th MUHURTA VAYU (Maruta, Samdram)','Lord of the Winds','en');
INSERT INTO "MUHURTA30_DESC" VALUES (31,1,'Рудра','1-я МУХУРТА РАУДРА','Владыка крика, рева, страха, потрясений, разрушений и перемен, изменений','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (32,2,'Ахи','2-я МУХУРТА АХИ','Владыка змей','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (33,3,'Митра','3-я МУХУРТА МАИТРА','Владыка дружелюбия','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (34,4,'Питри','4-я МУХУРТА ПИТРА','Владыка предков','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (35,5,'Васу','5-я МУХУРТА САВИТРА (ВАСУ)','Владыка жизненной силы','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (36,6,'Вайра','6-я МУХУРТА ВАЙРА (АМБУ)','Владыка вражды, мести, ссор, неприязни','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (37,7,'Вишва','7-я МУХУРТА ВИШВА (ВИШВАДЕВА)','Владыка цельности, полноты, единения, всеобщности','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (38,8,'Абхиджит','8-я МУХУРТА АБХИДЖИТ (Видхи)','Владыка победы и удачи','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (39,9,'Раухина','9-я МУХУРТА РАУХИНА (Видхата, Сатамукхи)','Владыка сандала','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (40,10,'Пурухута','10-я МУХУРТА БАЛА (Пурухута)','Владыка юности, молодости, детей','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (41,11,'Вахини','11-я МУХУРТА ВАХИ (Индраагни)','Владыка транспортировки','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (42,12,'Ниррити','12-я МУХУРТА НИРИТТА (Нактанчара)','Владыка смерти','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (43,13,'Варуна','13-я МУХУРТА ВАРУНА','Владыка воды','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (44,14,'Арйама','14-я МУХУРТА АРЙАМА','Владыка партнерства, товарищества, сватовства','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (45,15,'Бхага','15-я МУХУРТА БХАГА','Владыка благоденствия, процветания, удачи, семейного счастья','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (46,16,'Гириша','16-я МУХУРТА ГИРИША','Владыка гор','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (47,17,'Аджапада','17-я МУХУРТА АДЖАПАДА','Владыка первого шага','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (48,18,'Ахирбудхнйа','18-я МУХУРТА АХИРБУДХНЙА','Владыка подземных миров','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (49,19,'Пуша','19-я МУХУРТА ПУША','Владыка роста, процветания, стад и путешествий','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (50,20,'Ашви','20-я МУХУРТА АШВИ','Владыка врачевания и передвижений','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (51,21,'Йама','21-я МУХУРТА ЙАМА','Владыка ДХАРМЫ и смерти','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (52,22,'Агни','22-я МУХУРТА АГНИ','Владыка огня','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (53,23,'Видхатра','23-я МУХУРТА ВИДХАТРА','Владыка даров и распределения','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (54,24,'Чандра','24-я МУХУРТА РАТРИНАТХА (Чандра)','Владыка ночи','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (55,25,'Адити','25-я МУХУРТА АДИТЙА','Владыка благословений','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (56,26,'Джива','26-я МУХУРТА ДЖИВА','Владыка здоровья и жизненной силы','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (57,27,'Вишну','27-я МУХУРТА ВИШНУ','Владыки, Хранителя и Покровителя','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (58,28,'Тапас','28-я МУХУРТА ТАПАС (Йамигадути, Арка)','Владыка аскез','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (59,29,'Брахма','29-я МУХУРТА БРАХМА (Тйаштур)','Владыка созидания','ru');
INSERT INTO "MUHURTA30_DESC" VALUES (60,30,'Вайу','30-я МУХУРТА ВАЙУ (Марута, Самдрам)','Владыка ветров','ru');
INSERT INTO "MUHURTA_DESC" VALUES (1,1,'Abhijit Muhurta','AM','en');
INSERT INTO "MUHURTA_DESC" VALUES (2,2,'Rahu Kala','RK','en');
INSERT INTO "MUHURTA_DESC" VALUES (3,3,'Brahma Muhurta','BM','en');
INSERT INTO "MUHURTA_DESC" VALUES (4,4,'Gulika Kala','GK','en');
INSERT INTO "MUHURTA_DESC" VALUES (5,5,'Yamaganda','YG','en');
INSERT INTO "MUHURTA_DESC" VALUES (6,1,'Абхиджит-мухурта','АМ','ru');
INSERT INTO "MUHURTA_DESC" VALUES (7,2,'Раху-кала','РК','ru');
INSERT INTO "MUHURTA_DESC" VALUES (8,3,'Брахма-мухурта','БМ','ru');
INSERT INTO "MUHURTA_DESC" VALUES (9,4,'Гулика-кала','ГК','ru');
INSERT INTO "MUHURTA_DESC" VALUES (10,5,'Ямаганда','ЯГ','ru');
INSERT INTO "NAKSHATRA" VALUES (1,'ASHWINI',1);
INSERT INTO "NAKSHATRA" VALUES (2,'BHARANI',2);
INSERT INTO "NAKSHATRA" VALUES (3,'KRITTIKA',2);
INSERT INTO "NAKSHATRA" VALUES (4,'ROHINI',1);
INSERT INTO "NAKSHATRA" VALUES (5,'MRIGASHIRA',1);
INSERT INTO "NAKSHATRA" VALUES (6,'ARDRA',2);
INSERT INTO "NAKSHATRA" VALUES (7,'PUNARVASU',1);
INSERT INTO "NAKSHATRA" VALUES (8,'PUSHYA',1);
INSERT INTO "NAKSHATRA" VALUES (9,'ASHLESHA',2);
INSERT INTO "NAKSHATRA" VALUES (10,'MAGHA',2);
INSERT INTO "NAKSHATRA" VALUES (11,'PURVAPHALGUNI',2);
INSERT INTO "NAKSHATRA" VALUES (12,'UTTARAPHALGUNI',1);
INSERT INTO "NAKSHATRA" VALUES (13,'HASTA',1);
INSERT INTO "NAKSHATRA" VALUES (14,'CHITRA',1);
INSERT INTO "NAKSHATRA" VALUES (15,'SWATI',1);
INSERT INTO "NAKSHATRA" VALUES (16,'VISAKHA',1);
INSERT INTO "NAKSHATRA" VALUES (17,'ANURADHA',1);
INSERT INTO "NAKSHATRA" VALUES (18,'JYESHTHA',2);
INSERT INTO "NAKSHATRA" VALUES (19,'MULA',1);
INSERT INTO "NAKSHATRA" VALUES (20,'PURVAASHADHA',2);
INSERT INTO "NAKSHATRA" VALUES (21,'UTTARAASHADHA',1);
INSERT INTO "NAKSHATRA" VALUES (22,'SHRAVANA',1);
INSERT INTO "NAKSHATRA" VALUES (23,'DHANISHTA',1);
INSERT INTO "NAKSHATRA" VALUES (24,'SHATABHISHA',1);
INSERT INTO "NAKSHATRA" VALUES (25,'PURVABHADRAPADA',2);
INSERT INTO "NAKSHATRA" VALUES (26,'UTTARABHADRAPADA',1);
INSERT INTO "NAKSHATRA" VALUES (27,'REVATI',1);
INSERT INTO "NAKSHATRA_DESC" VALUES (1,1,'Ashwini','Ash','Ketu','Fast, sharp','Dynamic and active nakshatra bringing quick changes and fast progress. Good for beginnings and situations requiring speed.','Starts, movement, travel, healing, quick decisions.','Serious long-term commitments, slow important projects.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (2,1,'Ашвіні','Аш','Кету','Швидка, гостра','Динамічна накшатра, що приносить швидкі зміни та швидкий прогрес. Добра для починань і ситуацій, де потрібна швидкість.','Початки, рух, подорожі, лікування, швидкі рішення.','Серйозні довгострокові зобов’язання, повільні важливі проєкти.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (3,1,'Ashwini','Ash','Ketu','Szybka, ostra','Dynamiczna nakszatra przynosząca szybkie zmiany i szybki postęp. Dobra na początki i sytuacje wymagające szybkości.','Początki, ruch, podróże, uzdrawianie, szybkie decyzje.','Poważne zobowiązania długoterminowe, wolne projekty.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (4,1,'Ашвини','Аш','Кету','Быстрая, острая','Динамичная накшатра, дающая быстрые перемены и стремительный прогресс. Хороша для начинаний и всего, где нужна скорость.','Начинания, движение, поездки, лечение, быстрые решения.','Долгосрочные обязательства, медленные важные проекты.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (5,2,'Bharani','Bha','Venus','Strong, transformative','Powerful nakshatra associated with transformation, discipline and deep emotional processes.','Discipline, endurance, responsibility, transformation.','Pleasure-seeking, laziness, avoidance of duties.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (6,2,'Бхарані','Бха','Венера','Сильна, трансформаційна','Сильна накшатра, пов’язана з трансформацією, дисципліною та глибокими емоційними процесами.','Дисципліна, витривалість, відповідальність, трансформація.','Пошук задоволень, лінощі, уникнення обов’язків.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (7,2,'Bharani','Bha','Wenus','Silna, transformująca','Silna nakszatra związana z transformacją, dyscypliną i głębokimi emocjami.','Dyscyplina, wytrzymałość, odpowiedzialność, przemiana.','Unikanie obowiązków, lenistwo, szukanie przyjemności.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (8,2,'Бхарани','Бха','Венера','Сильная, трансформационная','Мощная накшатра, связанная с трансформацией, дисциплиной и глубокими эмоциональными процессами.','Дисциплина, выдержка, ответственность, преобразования.','Потворство удовольствиям, лень, избегание обязанностей.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (9,3,'Krittika','Kri','Sun','Sharp, fiery','Fiery nakshatra symbolizing purification, willpower, clarity and breakthrough energy.','Purification, cleaning, decisions, leadership, discipline.','Emotional impulsiveness, aggression, conflicts.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (10,3,'Крітікка','Крі','Сонце','Гостра, вогняна','Вогняна накшатра очищення, сили волі, ясності та проривної енергії.','Очищення, рішення, лідерство, дисципліна.','Імпульсивність, агресія, конфлікти.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (11,3,'Krittika','Kri','Słońce','Ostra, ognista','Ognista nakszatra oczyszczenia, siły woli i przełomu.','Oczyszczanie, decyzje, przywództwo, dyscyplina.','Impulsywność, agresja, konflikty.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (12,3,'Криттика','Кри','Солнце','Острая, огненная','Огненная накшатра очищения, силы воли, ясности и энергии прорыва.','Очищение, принятие решений, лидерство, дисциплина.','Импульсивность, агрессия, конфликты.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (13,4,'Rohini','Roh','Moon','Attractive, fertile','A nurturing nakshatra symbolizing beauty, growth, fertility and harmony. Supports prosperity and stability.','Relationships, beauty, creativity, comfort, finances.','Jealousy, overattachment, excessive indulgence.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (14,4,'Рохіні','Рох','Місяць','Приваблива, родюча','Плекаюча накшатра, що символізує красу, ріст, родючість і гармонію. Сприяє стабільності й процвітанню.','Стосунки, краса, творчість, комфорт, фінанси.','Ревнощі, надмірна прив’язаність, залежності.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (15,4,'Rohini','Roh','Księżyc','Atrakcyjna, płodna','Opiekuńcza nakszatra symbolizująca piękno, wzrost i harmonię. Sprzyja stabilności i dobrobytowi.','Relacje, piękno, kreatywność, komfort, finanse.','Zazdrość, nadmierne przywiązanie, rozpieszczanie.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (16,4,'Рохини','Рох','Луна','Привлекательная, плодородная','Питающая накшатра красоты, роста, плодородия и гармонии. Поддерживает процветание и стабильность.','Отношения, красота, творчество, комфорт, финансы.','Ревность, привязанность, излишние удовольствия.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (17,5,'Mrigashira','Mri','Mars','Searching, curious','A wandering and intellectual nakshatra connected with exploration, study and restlessness.','Research, travel, learning, exploration, communication.','Instability, indecision, nervousness.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (18,5,'Мрігашіра','Мрі','Марс','Пошукова, допитлива','Накшатра пошуку, дослідження й інтелектуальної активності. Пов’язана з рухом і зміною.','Дослідження, подорожі, навчання, спілкування.','Нестабільність, нерішучість, тривожність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (19,5,'Mrigashira','Mri','Mars','Poszukująca, ciekawa','Nakszatra związana z poszukiwaniem, badaniem i ruchem.','Badania, podróże, nauka, komunikacja.','Niestabilność, niezdecydowanie, nerwowość.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (20,5,'Мригашара','Мри','Марс','Ищущая, любознательная','Накшатра поиска, исследования и интеллектуальной активности. Связана с движением и переменами.','Исследования, поездки, обучение, общение.','Нестабильность, нерешительность, тревожность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (21,6,'Ardra','Ard','Rahu','Intense, transformative','A powerful nakshatra symbolizing storms, deep emotions, purification and necessary transformation.','Purification, breakthroughs, emotional release, research.','Conflicts, destructive actions, emotional instability.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (22,6,'Ардра','Ард','Раху','Інтенсивна, трансформаційна','Сильна накшатра глибоких емоцій, очищення і необхідних трансформацій.','Очищення, прориви, емоційне звільнення, дослідження.','Конфлікти, руйнівні дії, нестабільність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (23,6,'Ardra','Ard','Rahu','Intensywna, transformująca','Mocna nakszatra burz, emocji i oczyszczenia.','Oczyszczanie, przełomy, emocjonalne uwolnienie.','Konflikty, destrukcja, wahania emocjonalne.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (24,6,'Ардра','Ард','Раху','Интенсивная, трансформационная','Сильная накшатра бурь, очищения и глубоких эмоциональных перемен.','Очищение, прорывы, эмоциональное освобождение.','Конфликты, деструктивность, нестабильность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (25,7,'Punarvasu','Pun','Jupiter','Restoring, renewing','A bright and uplifting nakshatra symbolizing renewal, return of balance and a fresh start after difficulties.','Restoration, healing, education, travel, spiritual practices.','Instability at the beginning of projects, scattered focus.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (26,7,'Пунаравсу','Пун','Юпітер','Відновлююча, оновлююча','Світла накшатра оновлення, повернення балансу й нового початку після труднощів.','Відновлення, лікування, навчання, подорожі, духовні практики.','Нестабільність на початку проєктів, розсіяність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (27,7,'Punarvasu','Pun','Jowisz','Odnawiająca, przywracająca','Jasna nakszatra odnowy i powrotu równowagi po trudnościach.','Odnawianie, uzdrawianie, nauka, podróże, praktyki duchowe.','Niestabilność na początku, brak skupienia.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (28,7,'Пунарвасу','Пун','Юпитер','Восстанавливающая, обновляющая','Светлая накшатра обновления, возвращения баланса и нового начала после трудностей.','Восстановление, лечение, обучение, поездки, духовные практики.','Нестабильность в начале дел, рассеянность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (29,8,'Pushya','Pus','Saturn','Nourishing, disciplined','A deeply supportive nakshatra symbolizing nurturing, wisdom, structure and spiritual growth.','Education, discipline, service, responsibility, spiritual activities.','Rigidity, emotional coldness, excessive self-limitation.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (30,8,'Пушʼя','Пуш','Сатурн','Плекаюча, дисциплінована','Глибока підтримуюча накшатра, що символізує мудрість, структуру і духовне зростання.','Навчання, служіння, відповідальність, духовні практики.','Жорсткість, емоційна холодність, надмірні обмеження.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (31,8,'Pushya','Pus','Saturn','Wspierająca, zdyscyplinowana','Głęboko wspierająca nakszatra mądrości, struktury i duchowego wzrostu.','Nauka, służba, odpowiedzialność, praktyki duchowe.','Sztywność, chłód emocjonalny, nadmierne ograniczenia.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (32,8,'Пушья','Пуш','Сатурн','Питающая, дисциплинированная','Глубоко поддерживающая накшатра мудрости, структуры и духовного роста.','Учёба, служение, ответственность, духовные практики.','Жёсткость, эмоциональная холодность, излишние ограничения.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (33,9,'Ashlesha','Ashl','Mercury','Intense, binding','A mysterious nakshatra connected with intuition, psychology, depth, influence and transformation.','Research, strategy, psychology, healing of deep issues.','Manipulation, obsessions, toxic attachments.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (34,9,'Ашлеша','Ашл','Меркурій','Інтенсивна, зв’язуюча','Таємнича накшатра інтуїції, психології, впливу та трансформацій.','Дослідження, стратегія, психологія, глибинне зцілення.','Маніпуляції, залежності, токсичні прив’язаності.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (35,9,'Ashlesha','Ashl','Merkury','Intensywna, wiążąca','Tajemnicza nakszatra intuicji, psychologii i przemiany.','Badania, strategia, psychologia, głębokie uzdrawianie.','Manipulacje, obsesje, toksyczne więzi.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (36,9,'Ашлеша','Ашл','Меркурий','Интенсивная, связывающая','Таинственная накшатра интуиции, психологии, глубины и трансформаций.','Исследования, стратегия, психология, глубокое исцеление.','Манипуляции, навязчивости, токсичные привязанности.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (37,10,'Magha','Mag','Ketu','Royal, ancestral','A regal nakshatra associated with ancestors, dignity, status, authority and respect. Strengthens leadership qualities.','Leadership, status, authority, rituals, family heritage.','Pride, authoritarianism, attachment to status.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (38,10,'Магха','Маг','Кету','Королівська, родова','Королівська накшатра, пов’язана з предками, гідністю, статусом і владою. Посилює лідерські якості.','Лідерство, статус, авторитет, ритуали, спадщина.','Гордість, авторитарність, прив’язаність до статусу.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (39,10,'Magha','Mag','Ketu','Królewska, rodowa','Królewska nakszatra związana z przodkami, godnością i autorytetem. Wzmacnia cechy przywódcze.','Przywództwo, status, autorytet, rytuały, dziedzictwo.','Pycha, autorytaryzm, przywiązanie do statusu.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (40,10,'Магха','Маг','Кету','Королевская, родовая','Королевская накшатра, связанная с предками, достоинством, статусом и властью. Усиливает лидерские качества.','Лидерство, статус, авторитет, ритуалы, родовая линия.','Гордость, авторитарность, зависимость от статуса.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (41,11,'Purva Phalguni','PPha','Venus','Pleasant, joyful','A joyful nakshatra connected with pleasure, creativity, beauty, relationships and relaxation.','Creativity, relationships, celebration, art, enjoyment.','Excess, laziness, hedonism, avoidance of duties.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (42,11,'Пурва Пхалгуні','ППх','Венера','Приємна, радісна','Радісна накшатра творчості, кохання, краси та відпочинку.','Творчість, стосунки, свята, задоволення.','Надмірність, лінощі, гедонізм, уникнення обов’язків.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (43,11,'Purva Phalguni','PPha','Wenus','Przyjemna, radosna','Radosna nakszatra kreatywności, miłości i odpoczynku.','Kreatywność, relacje, świętowanie, przyjemności.','Nadmierna pobłażliwość, lenistwo, hedonizm.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (44,11,'Пурва Пхалгуни','ППх','Венера','Приятная, радостная','Радостная накшатра удовольствия, творчества, любви и отдыха.','Творчество, отношения, праздники, наслаждения.','Чрезмерность, лень, гедонизм, избегание обязанностей.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (45,12,'Uttara Phalguni','UPha','Sun','Supportive, friendly','A nakshatra of agreements, partnership, support, responsibility and mutual benefit.','Partnerships, contracts, cooperation, stability.','One-sided expectations, rigidity, dependency.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (46,12,'Уттара Пхалгуні','УПх','Сонце','Підтримуюча, дружня','Накшатра угод, партнерства, відповідальності та взаємної користі.','Партнерства, договори, співпраця, стабільність.','Односторонні очікування, жорсткість, залежність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (47,12,'Uttara Phalguni','UPha','Słońce','Wspierająca, przyjazna','Nakszatra umów, partnerstwa i stabilnej współpracy.','Partnerstwo, kontrakty, współpraca, stabilność.','Sztywność, zależność, nierówne oczekiwania.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (48,12,'Уттара Пхалгуни','УПх','Солнце','Поддерживающая, дружелюбная','Накшатра договорённостей, партнёрства, поддержки и стабильных связей.','Партнёрство, контракты, сотрудничество, стабильность.','Жёсткость, зависимость, завышенные ожидания.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (49,13,'Hasta','Has','Moon','Skillful, precise','A nakshatra of mastery, skill, control and the ability to shape reality. Supports crafts, communication and healing.','Craftsmanship, precision, communication, healing, learning.','Overcontrol, perfectionism, anxiety.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (50,13,'Хаста','Хас','Місяць','Уміла, точна','Накшатра майстерності, навичок, контролю та здатності формувати реальність.','Ремесло, точність, спілкування, зцілення, навчання.','Надмірний контроль, перфекціонізм, тривожність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (51,13,'Hasta','Has','Księżyc','Zręczna, precyzyjna','Nakszatra umiejętności, kontroli i kształtowania rzeczywistości.','Rzemiosło, precyzja, komunikacja, uzdrawianie, nauka.','Perfekcjonizm, nadmierna kontrola, lęk.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (52,13,'Хаста','Хас','Луна','Умелая, точная','Накшатра мастерства, навыка, контроля и умения формировать реальность.','Ремесло, точность, общение, лечение, обучение.','Перфекционизм, чрезмерный контроль, тревожность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (53,14,'Chitra','Chi','Mars','Creative, brilliant','A bright and artistic nakshatra symbolizing beauty, innovation, design and personal radiance.','Art, design, creativity, decoration, innovation.','Vanity, superficiality, impulsive decisions.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (54,14,'Чітра','Чі','Марс','Творча, яскрава','Яскрава художня накшатра краси, натхнення та інновацій.','Мистецтво, дизайн, творчість, естетика, нововведення.','Марнославство, поверховість, імпульсивність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (55,14,'Chitra','Chi','Mars','Twórcza, błyszcząca','Jasna, artystyczna nakszatra piękna i kreatywności.','Sztuka, projektowanie, kreatywność, estetyka.','Próżność, powierzchowność, impulsywność.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (56,14,'Читра','Чи','Марс','Творческая, яркая','Яркая художественная накшатра красоты, вдохновения и новизны.','Творчество, дизайн, эстетика, инновации.','Тщеславие, поверхностность, импульсивность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (57,15,'Swati','Swa','Rahu','Independent, flexible','A light, airy nakshatra of freedom, movement, independence and adaptability.','Independence, travel, learning, flexibility, communication.','Instability, inconsistency, avoidance of responsibility.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (58,15,'Сваті','Сва','Раху','Незалежна, гнучка','Легка повітряна накшатра свободи, руху, незалежності та адаптивності.','Незалежність, подорожі, навчання, гнучкість, комунікація.','Нестабільність, непослідовність, уникнення обов’язків.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (59,15,'Swati','Swa','Rahu','Niezależna, elastyczna','Lekka, powietrzna nakszatra wolności i ruchu.','Niezależność, podróże, nauka, elastyczność.','Niestabilność, unikanie odpowiedzialności.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (60,15,'Свати','Сва','Раху','Независимая, гибкая','Лёгкая воздушная накшатра свободы, движения и независимости.','Независимость, путешествия, обучение, гибкость.','Нестабильность, непоследовательность, избегание ответственности.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (61,16,'Vishakha','Vis','Jupiter','Goal-oriented, determined','A focused nakshatra symbolizing ambition, intensity, achievement and forward movement.','Goals, business, progress, discipline, competition.','Obsession, stubbornness, conflict with others.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (62,16,'Вішакха','Віш','Юпітер','Цілеспрямована, рішуча','Накшатра амбіцій, наполегливості й досягнення цілей.','Цілі, бізнес, прогрес, дисципліна, конкуренція.','Одержимість, упертість, конфлікти.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (63,16,'Vishakha','Vis','Jowisz','Skoncentrowana, zdecydowana','Nakszatra ambicji, determinacji i osiągnięć.','Cele, biznes, postęp, dyscyplina, rywalizacja.','Upór, obsesja, konflikty.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (64,16,'Вишакха','Виш','Юпитер','Целеустремлённая, решительная','Накшатра амбиций, силы воли и достижения целей.','Цели, бизнес, продвижение, дисциплина, соревнование.','Упрямство, одержимость, конфликты.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (65,17,'Anuradha','Anu','Saturn','Devoted, harmonious','A cooperative nakshatra connected with friendship, loyalty, teamwork and emotional balance.','Friendship, cooperation, spiritual progress, diplomacy.','Over-sensitivity, dependency, emotional fatigue.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (66,17,'Анурадха','Ану','Сатурн','Віддана, гармонійна','Накшатра дружби, лояльності, співпраці й внутрішньої рівноваги.','Дружба, співпраця, духовний розвиток, дипломатія.','Надчутливість, залежність, емоційне виснаження.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (67,17,'Anuradha','Anu','Saturn','Oddana, harmonijna','Nakszatra przyjaźni, współpracy i równowagi emocjonalnej.','Przyjaźń, współpraca, rozwój duchowy, dyplomacja.','Wrażliwość, zależność, zmęczenie emocjonalne.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (68,17,'Анурадха','Ану','Сатурн','Преданная, гармоничная','Накшатра дружбы, лояльности, сотрудничества и внутренней гармонии.','Дружба, сотрудничество, духовный рост, дипломатия.','Повышенная чувствительность, зависимость, эмоциональная усталость.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (69,18,'Jyeshtha','Jye','Mercury','Powerful, responsible','A mature nakshatra symbolizing leadership, protection, responsibility and personal strength.','Leadership, protection, responsibility, analysis.','Dominance, control issues, self-importance.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (70,18,'Джйєшта','Джє','Меркурій','Сильна, відповідальна','Зріла накшатра лідерства, захисту й відповідальності.','Лідерство, захист, аналіз, відповідальність.','Домінування, контроль, завищене его.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (71,18,'Jyeshtha','Jye','Merkury','Silna, odpowiedzialna','Dojrzała nakszatra odpowiedzialności i ochrony.','Przywództwo, ochrona, analiza, odpowiedzialność.','Dominacja, kontrola, poczucie wyższości.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (72,18,'Джйештха','Дже','Меркурий','Сильная, ответственная','Зрелая накшатра силы, защиты, ответственности и лидерства.','Лидерство, защита, анализ, ответственность.','Доминирование, контроль, завышенная значимость.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (73,19,'Mula','Mul','Ketu','Deep, root-cutting','A nakshatra of roots, truth, destruction of illusions and deep transformation. Helps reach the core of any issue.','Research, psychological work, surgery, transformations.','Destruction, chaos, emotional instability.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (74,19,'Мула','Мул','Кету','Глибока, руйнуюча','Накшатра коренів, істини та глибоких трансформацій. Допомагає доходити до суті будь-якої проблеми.','Дослідження, психологія, трансформації, хірургія.','Руйнування, хаос, емоційна нестабільність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (75,19,'Mula','Mul','Ketu','Głęboka, transformująca','Nakszatra korzeni, prawdy i głębokiej przemiany. Pomaga dotrzeć do sedna problemu.','Badania, psychologia, przemiana, zabiegi.','Chaos, destrukcja, niestabilność emocjonalna.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (76,19,'Мула','Мул','Кету','Глубокая, корневая','Накшатра корней, истины и разрушения иллюзий, ведущая к трансформации.','Исследования, психология, хирургия, преобразования.','Хаос, разрушение, эмоциональная нестабильность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (77,20,'Purva Ashadha','PAsh','Venus','Victorious, uplifting','A victorious nakshatra of motivation, cleansing, enthusiasm and inner strength. Supports growth and confidence.','Inspiration, motivation, cleansing, creativity, progress.','Arrogance, stubborn ideals, emotional pressure.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (78,20,'Пурва Ашадха','ПАш','Венера','Переможна, надихаюча','Накшатра мотивації, очищення, сили та натхнення. Сприяє зростанню й упевненості.','Натхнення, мотивація, очищення, творчість, прогрес.','Зарозумілість, уперті ідеали, емоційний тиск.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (79,20,'Purva Ashadha','PAsh','Wenus','Zwycięska, inspirująca','Nakszatra inspiracji, oczyszczenia i wewnętrznej siły.','Inspiracja, oczyszczanie, kreatywność, rozwój.','Arogancja, upór, presja emocjonalna.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (80,20,'Пурва Ашадха','ПАш','Венера','Победная, вдохновляющая','Накшатра вдохновения, очищения, силы и роста. Поддерживает уверенность и развитие.','Вдохновение, мотивация, очищение, творчество.','Заносчивость, упёртость, эмоциональное давление.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (81,21,'Uttara Ashadha','UAsh','Sun','Victorious, firm','A powerful nakshatra symbolizing long-term success, dignity, stability and righteous action.','Success, leadership, long-term projects, agreements.','Rigidity, excessive responsibility, inflexibility.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (82,21,'Уттара Ашадха','УАш','Сонце','Переможна, стійка','Сильна накшатра довготривалого успіху, гідності та стабільності.','Успіх, лідерство, угоди, довготривалі справи.','Жорсткість, надмірна відповідальність, негнучкість.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (83,21,'Uttara Ashadha','UAsh','Słońce','Zwycięska, stabilna','Silna nakszatra stabilności i długofalowego sukcesu.','Sukces, przywództwo, projekty długoterminowe.','Sztywność, przeciążenie obowiązkami.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (84,21,'Уттара Ашадха','УАш','Солнце','Победная, устойчивая','Сильная накшатра устойчивого успеха, стабильности и благородства действия.','Успех, лидерство, договоры, долгосрочные дела.','Жёсткость, избыточная ответственность, негибкость.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (85,22,'Shravana','Shr','Moon','Listening, learning','A wise nakshatra associated with knowledge, listening, communication, learning and understanding.','Learning, studying, communication, consulting, planning.','Overthinking, worry, passivity.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (86,22,'Шравана','Шра','Місяць','Слухання, навчання','Мудра накшатра знань, слухання, комунікації та розуміння.','Навчання, консультації, комунікація, планування.','Надмірні думки, тривожність, пасивність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (87,22,'Shravana','Shr','Księżyc','Słuchanie, nauka','Mądra nakszatra wiedzy, komunikacji i planowania.','Nauka, doradztwo, komunikacja, planowanie.','Zamartwianie się, bierność.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (88,22,'Шравана','Шра','Луна','Слушание, обучение','Мудрая накшатра знаний, умения слушать, учиться и понимать.','Учёба, консультации, общение, планирование.','Переживания, чрезмерные мысли, пассивность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (89,23,'Dhanishta','Dha','Mars','Rhythmic, prosperous','A rhythmic nakshatra linked with music, prosperity, success, teamwork and abundance.','Music, teamwork, finances, social activity, progress.','Pride, impulsiveness, excessive ambition.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (90,23,'Дханіштха','Дха','Марс','Ритмічна, процвітаюча','Ритмічна накшатра музики, процвітання, командної роботи та успіху.','Музика, команда, фінанси, соціальна активність.','Гордість, імпульсивність, надмірні амбіції.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (91,23,'Dhanishta','Dha','Mars','Rytmiczna, dostatnia','Rytmiczna nakszatra muzyki, zespołowości i dobrobytu.','Muzyka, współpraca, finanse, aktywność społeczna.','Pycha, impulsywność, ambicja.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (92,23,'Дхаништха','Дха','Марс','Ритмичная, процветающая','Ритмичная накшатра музыки, богатства, сотрудничества и успеха.','Музыка, команда, финансы, социальная активность.','Гордость, импульсивность, избыточные амбиции.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (93,24,'Shatabhisha','Sha','Rahu','Healing, secretive','A mystical nakshatra connected with healing, introspection, research and hidden knowledge.','Healing, research, meditation, solitude, analysis.','Isolation, emotional coldness, overcriticism.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (94,24,'Сатабхіша','Сат','Раху','Цілюща, таємнича','Містична накшатра лікування, досліджень та внутрішнього аналізу.','Зцілення, дослідження, медитація, усамітнення.','Ізоляція, емоційний холод, критичність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (95,24,'Shatabhisha','Sha','Rahu','Uzdrawiająca, tajemnicza','Mistyczna nakszatra uzdrawiania, badań i introspekcji.','Uzdrawianie, badania, medytacja, samotność.','Izolacja, chłód emocjonalny, krytycyzm.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (96,24,'Сатабхиша','Сат','Раху','Лечебная, тайная','Мистическая накшатра лечения, исследований и внутренней работы.','Исцеление, исследования, медитация, уединение.','Изоляция, холодность, излишняя критичность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (97,25,'Purva Bhadrapada','PBha','Jupiter','Intense, mystical','A deep nakshatra linked with mysticism, intensity, inner fire, transformation and strong convictions.','Spiritual work, transformation, deep study, rituals.','Fanaticism, extremism, excessive intensity.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (98,25,'Пурва Бхадрапада','ПБх','Юпітер','Інтенсивна, містична','Глибока накшатра містики, внутрішнього вогню, трансформацій та сильних переконань.','Духовна робота, трансформації, глибинне навчання, ритуали.','Фанатизм, крайнощі, надмірна інтенсивність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (99,25,'Purva Bhadrapada','PBha','Jowisz','Intensywna, mistyczna','Głęboka nakszatra mistycyzmu, przemiany i wewnętrznej siły.','Praca duchowa, przemiana, głęboka nauka, rytuały.','Fanatyzm, skrajności, zbyt duża intensywność.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (100,25,'Пурва Бхадрапада','ПБх','Юпитер','Интенсивная, мистическая','Глубокая накшатра мистики, внутреннего огня, трансформаций и сильных убеждений.','Духовная работа, трансформации, глубокое обучение, ритуалы.','Фанатизм, крайности, чрезмерная интенсивность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (101,26,'Uttara Bhadrapada','UBha','Saturn','Stable, deep','A calm and profound nakshatra symbolizing inner stability, wisdom, emotional depth and spiritual maturity.','Meditation, stability, deep understanding, long-term work.','Isolation, melancholy, excessive seriousness.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (102,26,'Уттара Бхадрапада','УБх','Сатурн','Стабільна, глибока','Спокійна глибока накшатра внутрішньої стабільності, мудрості та зрілості.','Медитація, стабільність, глибоке розуміння, тривала робота.','Ізоляція, меланхолія, надмірна серйозність.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (103,26,'Uttara Bhadrapada','UBha','Saturn','Stabilna, głęboka','Spokojna i głęboka nakszatra mądrości i wewnętrznej stabilności.','Medytacja, stabilność, głębokie zrozumienie.','Izolacja, melancholia, nadmierna powaga.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (104,26,'Уттара Бхадрапада','УБх','Сатурн','Стабильная, глубокая','Спокойная глубокая накшатра внутренней стабильности, мудрости и зрелости.','Медитация, стабильность, глубокое понимание, длительная работа.','Изоляция, меланхолия, чрезмерная серьёзность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (105,27,'Revati','Rev','Mercury','Gentle, guiding','A gentle and compassionate nakshatra symbolizing protection, guidance, completion and support for others.','Compassion, travel, completion of projects, guidance, care.','Overgiving, emotional exhaustion, indecision.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (106,27,'Реваті','Рев','Меркурій','Мʼяка, провідна','Мʼяка співчутлива накшатра захисту, підтримки та завершення справ.','Співчуття, подорожі, завершення проєктів, допомога.','Емоційне виснаження, нерішучість, надмірна турбота.','uk');
INSERT INTO "NAKSHATRA_DESC" VALUES (107,27,'Revati','Rev','Merkury','Łagodna, prowadząca','Łagodna nakszatra opieki, wsparcia i pomyślnego zakończenia spraw.','Opieka, współczucie, podróże, finalizowanie projektów.','Wyczerpanie emocjonalne, niezdecydowanie.','pl');
INSERT INTO "NAKSHATRA_DESC" VALUES (108,27,'Ревати','Рев','Меркурий','Нежная, ведущая','Мягкая сострадательная накшатра защиты, завершения и поддержки.','Сострадание, путешествия, завершение дел, помощь.','Эмоциональная усталость, нерешительность.','ru');
INSERT INTO "NITYAYOGA" VALUES (1,'VISHKUMBHA',2,8,7,2);
INSERT INTO "NITYAYOGA" VALUES (2,'PRITI',1,9,4,3);
INSERT INTO "NITYAYOGA" VALUES (3,'AYUSHMAAN',1,10,9,8);
INSERT INTO "NITYAYOGA" VALUES (4,'SAUBHAGYA',1,11,6,5);
INSERT INTO "NITYAYOGA" VALUES (5,'SHOBHANA',1,12,1,7);
INSERT INTO "NITYAYOGA" VALUES (6,'ATIGANDA',2,13,2,4);
INSERT INTO "NITYAYOGA" VALUES (7,'SUKARMAA',1,14,3,9);
INSERT INTO "NITYAYOGA" VALUES (8,'DHRITI',1,15,8,6);
INSERT INTO "NITYAYOGA" VALUES (9,'SHULA',2,16,5,1);
INSERT INTO "NITYAYOGA" VALUES (10,'GANDA',2,17,7,2);
INSERT INTO "NITYAYOGA" VALUES (11,'VRIDDHI',1,18,4,3);
INSERT INTO "NITYAYOGA" VALUES (12,'DHRUVA',1,19,9,8);
INSERT INTO "NITYAYOGA" VALUES (13,'VYAGHATA',2,20,6,5);
INSERT INTO "NITYAYOGA" VALUES (14,'HARSHANA',1,21,1,7);
INSERT INTO "NITYAYOGA" VALUES (15,'VAJRA',2,22,2,4);
INSERT INTO "NITYAYOGA" VALUES (16,'SIDDHI',1,23,3,9);
INSERT INTO "NITYAYOGA" VALUES (17,'VYATIPAATA',2,24,8,6);
INSERT INTO "NITYAYOGA" VALUES (18,'VARIYANA',1,25,5,1);
INSERT INTO "NITYAYOGA" VALUES (19,'PARIGHA',2,26,7,2);
INSERT INTO "NITYAYOGA" VALUES (20,'SHIVA',1,27,4,3);
INSERT INTO "NITYAYOGA" VALUES (21,'SIDDHA',1,1,9,8);
INSERT INTO "NITYAYOGA" VALUES (22,'SADHYA',1,2,6,5);
INSERT INTO "NITYAYOGA" VALUES (23,'SHUBHA',1,3,1,7);
INSERT INTO "NITYAYOGA" VALUES (24,'SHUKLA',1,4,2,4);
INSERT INTO "NITYAYOGA" VALUES (25,'BRAHMA',1,5,3,9);
INSERT INTO "NITYAYOGA" VALUES (26,'INDRA',1,6,8,6);
INSERT INTO "NITYAYOGA" VALUES (27,'VAIDHRITI',2,7,5,1);
INSERT INTO "NITYAYOGA_DESC" VALUES (1,1,'Vishkumbha','Yama','Poisoned pot','Inauspicious yoga, harsh and restrictive. Favorable only for conflicts, competitions, healing and purification. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (2,1,'Вишкумбха','Яма','Горшок с ядом','Неблагоприятная йога, жёсткая и напряжённая. Благоприятна только для конфликтов, соревнований, процедур очищения и здоровья. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (3,1,'Вішкумбха','Яма','Горщик з отрутою','Несприятлива йога, жорстка та напружена. Добра лише для конфліктів, змагань, очищення й лікування. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (4,1,'Vishkumbha','Yama','Garnek z trucizną','Niekorzystna, twarda joga. Dobra tylko do konfliktów, rywalizacji i oczyszczania. Nie planować ważnych spraw.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (5,2,'Priti','Vishnu','Joy, affection','Auspicious for meetings, communication, relationships, social events and partnerships.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (6,2,'Прити','Вишну','Возлюбленный, радость','Благоприятна для знакомств, контактов, отношений, общественных событий и брака.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (7,2,'Пріті','Вішну','Радість, прихильність','Сприятлива для знайомств, контактів, стосунків і соціальних подій.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (8,2,'Priti','Vishnu','Radość, uczucie','Pomyślna dla spotkań, kontaktów i relacji.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (9,3,'Ayushman','Chandra','Longevity','Excellent for health, longevity practices, rejuvenation, legal actions and politics.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (10,3,'Айюшман','Чандра','Долгоживущий','Идеальна для всего, что связано со здоровьем, долголетием, омоложением, правом и политикой.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (11,3,'Аюшман','Чандра','Довголіття','Чудова для здоров’я, омолодження, юридичних справ і політики.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (12,3,'Ayushman','Chandra','Długowieczność','Doskonała dla zdrowia, długowieczności i działań prawnych.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (13,4,'Saubhagya','Shukra','Fortune','Auspicious for joy, relationships, creativity, purchases and celebrations.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (14,4,'Саубхагья','Шукра','Счастливая','Благоприятна для радостных дел, отношений, творчества, покупок, праздников.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (15,4,'Саубхагья','Шукра','Щаслива','Добра для радості, стосунків, творчості та покупок.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (16,4,'Saubhagya','Shukra','Szczęśliwość','Pomyślna dla relacji, twórczości i zakupów.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (17,5,'Shobhana','Manmadha','Beauty','Good for romance, pleasant purchases, art, travel and new beginnings.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (18,5,'Шобхана','Манмадха','Благоприятный','Хороша для романтики, искусства, путешествий, удачных покупок и начала дел.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (19,5,'Шобхана','Манмадха','Гарна','Сприятлива для романтики, мистецтва, подорожей і нових починань.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (20,5,'Shobhana','Manmadha','Piękno','Dobra dla romansu, sztuki, podróży i zakupów.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (21,6,'Atiganda','Agni','Strong knot','Highly inauspicious. Conflicts, troubles, obstacles. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (22,6,'Атиганда','Агни','Сильный узел','Очень неблагоприятная. Конфликты, проблемы, сложности. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (23,6,'Атіганда','Агні','Сильний вузол','Дуже несприятлива. Конфлікти, труднощі. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (24,6,'Atiganda','Agni','Silny węzeł','Bardzo niekorzystna. Konflikty i trudności. Nie planować ważnych spraw.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (25,7,'Sukarma','Indra','Right action','Good for work, effort, improvement and professional progress.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (26,7,'Сукарма','Индра','Правильное действие','Благоприятна для труда, работы, профессиональных усилий и улучшений.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (27,7,'Сукарма','Індра','Правильна дія','Добра для роботи, зусиль та професійного росту.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (28,7,'Sukarma','Indra','Właściwe działanie','Pomyślna dla pracy i rozwoju.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (29,8,'Dhriti','Vayu','Stability','Good for endurance, strength, steady decisions and strengthening foundations.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (30,8,'Дхрити','Ваю','Устойчивость','Благоприятна для стабильности, силы, устойчивых решений.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (31,8,'Дхріті','Ваю','Стійкість','Добра для стійкості, рішучості й зміцнення позицій.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (32,8,'Dhriti','Vayu','Stabilność','Dobra dla wytrwałości i stabilności.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (33,9,'Shula','Shakti','Spear','Inauspicious. Misunderstandings, harsh actions, mistakes. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (34,9,'Шула','Шакти','Копьё','Неблагоприятная. Непонимание, ошибки, резкие действия. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (35,9,'Шула','Шакті','Спис','Несприятлива. Непорозуміння, помилки. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (36,9,'Shula','Shakti','Włócznia','Niekorzystna. Konflikty i błędy. Nie planować ważnych spraw.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (37,10,'Ganda','Agni','Knot, tangle','Inauspicious. Fraud, confusion, tangled situations. Avoid trust-based deals. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (38,10,'Ганда','Агни','Узел','Неблагоприятная. Обман, запутанные ситуации. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (39,10,'Ганда','Агні','Вузол','Несприятлива. Обман і плутанина. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (40,10,'Ganda','Agni','Węzeł','Niekorzystna. Zamieszanie i oszustwa. Nie planować ważnych spraw.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (41,11,'Vriddhi','Surya','Growth','Auspicious for success, prosperity and overcoming obstacles.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (42,11,'Вриддхи','Сурья','Рост','Благоприятна для успеха, роста и преодоления препятствий.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (43,11,'Вріддгі','Сурʼя','Зростання','Добра для успіху й розвитку.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (44,11,'Vriddhi','Surya','Wzrost','Pomyślna dla sukcesu.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (45,12,'Dhruva','Bhumi','Steady','Very auspicious. Ideal for marriage, contracts and long-term projects.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (46,12,'Дхрува','Бхуми','Стабильный','Благоприятная. Отлична для брака, контрактов, долгосрочных дел.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (47,12,'Дхрува','Бхумі','Стійкий','Сприятлива для шлюбу, угод і довгострокових справ.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (48,12,'Dhruva','Bhumi','Stały','Doskonała dla trwałych przedsięwzięć.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (49,13,'Vyaghata','Vayu','Harmful obstacle','Highly inauspicious. Conflicts, danger. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (50,13,'Вьягхата','Вайю','Препятствие','Очень неблагоприятная. Конфликты, опасные ситуации. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (51,13,'Вʼягхата','Ваю','Перешкода','Дуже несприятлива. Конфлікти й небезпека. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (52,13,'Vyaghata','Vayu','Przeszkoda','Bardzo niekorzystna. Nie planować ważnych spraw.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (53,14,'Harshana','Bhaga','Joy','Good for relationships, romance and overcoming difficulties.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (54,14,'Харшана','Бхага','Радостный','Хороша для отношений, романтики и преодоления трудностей.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (55,14,'Харшана','Бгага','Радісний','Добра для стосунків і радості.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (56,14,'Harshana','Bhaga','Radość','Pomyślna dla relacji.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (57,15,'Vajra','Varuna','Thunderbolt','Inauspicious. Conflicts, harsh methods. Good for protection. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (58,15,'Ваджра','Варуна','Удар молнии','Неблагоприятная. Жёсткие конфликты. Хороша для защиты. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (59,15,'Ваджра','Варуна','Блискавка','Несприятлива. Конфлікти. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (60,15,'Vajra','Varuna','Piorun','Niekorzystna. Nie planować ważnych spraw.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (61,16,'Siddhi','Ganesha','Success','Very auspicious. Supports success even in difficult situations.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (62,16,'Сиддхи','Ганеша','Завершённость','Очень благоприятна. Помогает в трудных обстоятельствах.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (63,16,'Сіддгі','Ґанеша','Успіх','Дуже сприятлива й сильна.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (64,16,'Siddhi','Ganesha','Sukces','Bardzo pomyślna.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (65,17,'Vyatipata','Rudra','Misfortune','Extremely inauspicious. Danger and disasters. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (66,17,'Вьятипаата','Рудра','Несчастье','Очень неблагоприятная. Опасность. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (67,17,'Вʼятіпата','Рудра','Лихо','Дуже несприятлива. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (68,17,'Vyatipata','Rudra','Nieszczęście','Skrajnie niekorzystna. Nie planować ważnych spraw.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (69,18,'Variyana','Kubera','Comfort','Auspicious for purchases, luxury and relationships.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (70,18,'Варийана','Кубера','Комфорт','Хорошая йога для покупки дорогих вещей и любовных дел.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (71,18,'Варіяна','Кубера','Комфорт','Добра для покупок і кохання.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (72,18,'Variyana','Kubera','Komfort','Pomyślna dla zakupów.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (73,19,'Parigha','Vishvakarma','Iron bar','Inauspicious. Obstacles, blocks, delays. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (74,19,'Паригха','Вишвакарма','Запор','Неблагоприятная. Препятствия. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (75,19,'Паріґха','Вішвакарма','Перешкода','Несприятлива. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (76,19,'Parigha','Vishvakarma','Blokada','Niekorzystna. Nie planować ważnych spraw.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (77,20,'Shiva','Mitra','Auspicious','Good for leadership, authority, learning and earnings.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (78,20,'Шива','Митра','Благоприятный','Хороша для лидерства, власти, обучения и заработка.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (79,20,'Шіва','Мітра','Сприятливий','Добрий час для влади, навчання й доходів.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (80,20,'Shiva','Mitra','Pomyślny','Dobry na naukę i rozwój.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (81,21,'Siddha','Karttikeya','Accomplished','Auspicious for achieving strong, successful results.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (82,21,'Сиддха','Картикея','Завершённый','Благоприятна для получения максимальных результатов.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (83,21,'Сіддха','Картікея','Завершений','Сприятлива для успіху.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (84,21,'Siddha','Karttikeya','Zakończony','Sprzyja osiąganiu efektów.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (85,22,'Sadhya','Savitri','Friendly','Good for diplomacy, communication, agreements and peaceful solutions.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (86,22,'Садхья','Савитри','Дружественный','Хороша для дипломатии, переговоров и компромиссов.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (87,22,'Садгʼя','Савітрі','Дружній','Добра для переговорів і згоди.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (88,22,'Sadhya','Savitri','Przyjazny','Dobry dla dyplomacji.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (89,23,'Shubha','Indra','Good, auspicious','Excellent yoga for positive activities.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (90,23,'Шубха','Индра','Благой','Отличная йога для позитивных дел.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (91,23,'Шубха','Індра','Добрий','Чудова для добрих справ.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (92,23,'Shubha','Indra','Dobry','Bardzo pomyślna.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (93,24,'Shukla','Vishnu','Bright','Auspicious for beginnings, creativity, relationships and purchases.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (94,24,'Шукла','Вишну','Светлый','Благоприятна для начинаний, творчества, отношений и покупок.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (95,24,'Шукла','Вішну','Світлий','Сприятлива для починань і творчості.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (96,24,'Shukla','Vishnu','Jasny','Sprzyja nowym początkom.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (97,25,'Brahma','Brahma','Creation','Good for study, creativity, construction and new projects.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (98,25,'Брахма','Брахма','Созидание','Хороша для учёбы, творчества, строительства и начинаний.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (99,25,'Брагма','Брагма','Творення','Добра для навчання й створення.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (100,25,'Brahma','Brahma','Stworzenie','Dobra dla nauki i tworzenia.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (101,26,'Indra','Pitri','Lord of Devas','Good for study, learning and creating new things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (102,26,'Индра','Питри','Царь Девов','Хороша для учёбы, изучения и создания нового.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (103,26,'Індра','Пітрі','Володар Девів','Добра для навчання й розвитку.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (104,26,'Indra','Pitri','Władca Dewów','Dobra dla nauki.','pl');
INSERT INTO "NITYAYOGA_DESC" VALUES (105,27,'Vaidhriti','Diti','Delay, stagnation','Extremely inauspicious. No support. Suitable only for complaints or behind-the-scenes actions. Do not plan important matters.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (106,27,'Вайдхрити','Дити','Задержка','Очень неблагоприятная. Нет поддержки. Подходит только для жалоб и закулисных действий. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (107,27,'Вайдхріті','Діті','Затримка','Дуже несприятлива. Підтримки нема. Не планувати важливі справи.','uk');
INSERT INTO "NITYAYOGA_DESC" VALUES (108,27,'Vaidhriti','Diti','Zastój','Skrajnie niekorzystna. Nie planować ważnych spraw.','pl');
INSERT INTO "PADA" VALUES (1,1,1,1,1,'3,2,8',1,2);
INSERT INTO "PADA" VALUES (2,1,1,2,1,'',2,3);
INSERT INTO "PADA" VALUES (3,1,1,3,1,'',3,3);
INSERT INTO "PADA" VALUES (4,1,1,4,2,'',4,3);
INSERT INTO "PADA" VALUES (5,1,2,1,2,'',5,3);
INSERT INTO "PADA" VALUES (6,1,2,2,2,'',6,3);
INSERT INTO "PADA" VALUES (7,1,2,3,3,'1',7,1);
INSERT INTO "PADA" VALUES (8,1,2,4,3,'7',8,3);
INSERT INTO "PADA" VALUES (9,1,3,1,3,'1',9,1);
INSERT INTO "PADA" VALUES (10,2,3,2,4,'2',10,5);
INSERT INTO "PADA" VALUES (11,2,3,3,4,'',11,3);
INSERT INTO "PADA" VALUES (12,2,3,4,4,'1',12,1);
INSERT INTO "PADA" VALUES (13,2,4,1,5,'4',1,5);
INSERT INTO "PADA" VALUES (14,2,4,2,5,'4,1,8',2,3);
INSERT INTO "PADA" VALUES (15,2,4,3,5,'4',3,5);
INSERT INTO "PADA" VALUES (16,2,4,4,6,'',4,3);
INSERT INTO "PADA" VALUES (17,2,5,1,6,'',5,3);
INSERT INTO "PADA" VALUES (18,2,5,2,6,'',6,3);
INSERT INTO "PADA" VALUES (19,3,5,3,7,'',7,3);
INSERT INTO "PADA" VALUES (20,3,5,4,7,'',8,3);
INSERT INTO "PADA" VALUES (21,3,6,1,7,'',9,3);
INSERT INTO "PADA" VALUES (22,3,6,2,8,'7',10,3);
INSERT INTO "PADA" VALUES (23,3,6,3,8,'2',11,5);
INSERT INTO "PADA" VALUES (24,3,6,4,8,'1',12,1);
INSERT INTO "PADA" VALUES (25,3,7,1,9,'',1,3);
INSERT INTO "PADA" VALUES (26,3,7,2,9,'1',2,1);
INSERT INTO "PADA" VALUES (27,3,7,3,9,'8',3,3);
INSERT INTO "PADA" VALUES (28,4,7,4,10,'1,8',4,1);
INSERT INTO "PADA" VALUES (29,4,8,1,10,'',5,3);
INSERT INTO "PADA" VALUES (30,4,8,2,10,'1',6,1);
INSERT INTO "PADA" VALUES (31,4,8,3,11,'4',7,5);
INSERT INTO "PADA" VALUES (32,4,8,4,11,'4',8,5);
INSERT INTO "PADA" VALUES (33,4,9,1,11,'4',9,5);
INSERT INTO "PADA" VALUES (34,4,9,2,12,'4',10,5);
INSERT INTO "PADA" VALUES (35,4,9,3,12,'4,7',11,5);
INSERT INTO "PADA" VALUES (36,4,9,4,12,'4,2,3',12,2);
INSERT INTO "PADA" VALUES (37,5,10,1,13,'4,3',1,2);
INSERT INTO "PADA" VALUES (38,5,10,2,13,'',2,3);
INSERT INTO "PADA" VALUES (39,5,10,3,13,'',3,3);
INSERT INTO "PADA" VALUES (40,5,10,4,14,'',4,3);
INSERT INTO "PADA" VALUES (41,5,11,1,14,'2,8',5,5);
INSERT INTO "PADA" VALUES (42,5,11,2,14,'',6,3);
INSERT INTO "PADA" VALUES (43,5,11,3,15,'1',7,1);
INSERT INTO "PADA" VALUES (44,5,11,4,15,'',8,3);
INSERT INTO "PADA" VALUES (45,5,12,1,15,'1',9,1);
INSERT INTO "PADA" VALUES (46,6,12,2,16,'2',10,5);
INSERT INTO "PADA" VALUES (47,6,12,3,16,'',11,3);
INSERT INTO "PADA" VALUES (48,6,12,4,16,'1',12,1);
INSERT INTO "PADA" VALUES (49,6,13,1,17,'7',1,3);
INSERT INTO "PADA" VALUES (50,6,13,2,17,'1',2,1);
INSERT INTO "PADA" VALUES (51,6,13,3,17,'',3,3);
INSERT INTO "PADA" VALUES (52,6,13,4,18,'',4,3);
INSERT INTO "PADA" VALUES (53,6,14,1,18,'',5,3);
INSERT INTO "PADA" VALUES (54,6,14,2,18,'8',6,3);
INSERT INTO "PADA" VALUES (55,7,14,3,19,'8',7,3);
INSERT INTO "PADA" VALUES (56,7,14,4,19,'',8,3);
INSERT INTO "PADA" VALUES (57,7,15,1,19,'',9,3);
INSERT INTO "PADA" VALUES (58,7,15,2,20,'',10,3);
INSERT INTO "PADA" VALUES (59,7,15,3,20,'2',11,5);
INSERT INTO "PADA" VALUES (60,7,15,4,20,'1',12,1);
INSERT INTO "PADA" VALUES (61,7,16,1,21,'',1,3);
INSERT INTO "PADA" VALUES (62,7,16,2,21,'1,7',2,1);
INSERT INTO "PADA" VALUES (63,7,16,3,21,'',3,3);
INSERT INTO "PADA" VALUES (64,8,16,4,22,'1,4',4,3);
INSERT INTO "PADA" VALUES (65,8,17,1,22,'4',5,5);
INSERT INTO "PADA" VALUES (66,8,17,2,22,'4,1',6,3);
INSERT INTO "PADA" VALUES (67,8,17,3,23,'4,5',7,5);
INSERT INTO "PADA" VALUES (68,8,17,4,23,'8,4,5',8,5);
INSERT INTO "PADA" VALUES (69,8,18,1,23,'4,5',9,5);
INSERT INTO "PADA" VALUES (70,8,18,2,24,'',10,3);
INSERT INTO "PADA" VALUES (71,8,18,3,24,'',11,3);
INSERT INTO "PADA" VALUES (72,8,18,4,24,'2,3',12,2);
INSERT INTO "PADA" VALUES (73,9,19,1,25,'2,3',1,2);
INSERT INTO "PADA" VALUES (74,9,19,2,25,'',2,3);
INSERT INTO "PADA" VALUES (75,9,19,3,25,'',3,3);
INSERT INTO "PADA" VALUES (76,9,19,4,26,'7',4,3);
INSERT INTO "PADA" VALUES (77,9,20,1,26,'',5,3);
INSERT INTO "PADA" VALUES (78,9,20,2,26,'',6,3);
INSERT INTO "PADA" VALUES (79,9,20,3,27,'1',7,1);
INSERT INTO "PADA" VALUES (80,9,20,4,27,'',8,3);
INSERT INTO "PADA" VALUES (81,9,21,1,27,'1,8',9,1);
INSERT INTO "PADA" VALUES (82,10,21,2,28,'8,6',10,5);
INSERT INTO "PADA" VALUES (83,10,21,3,28,'6',11,5);
INSERT INTO "PADA" VALUES (84,10,21,4,28,'6,1',12,3);
INSERT INTO "PADA" VALUES (85,10,22,1,29,'',1,3);
INSERT INTO "PADA" VALUES (86,10,22,2,29,'1',2,1);
INSERT INTO "PADA" VALUES (87,10,22,3,29,'',3,3);
INSERT INTO "PADA" VALUES (88,10,22,4,30,'',4,3);
INSERT INTO "PADA" VALUES (89,10,23,1,30,'7',5,3);
INSERT INTO "PADA" VALUES (90,10,23,2,30,'2',6,5);
INSERT INTO "PADA" VALUES (91,11,23,3,31,'',7,3);
INSERT INTO "PADA" VALUES (92,11,23,4,31,'',8,3);
INSERT INTO "PADA" VALUES (93,11,24,1,31,'',9,3);
INSERT INTO "PADA" VALUES (94,11,24,2,32,'',10,3);
INSERT INTO "PADA" VALUES (95,11,24,3,32,'2,8',11,5);
INSERT INTO "PADA" VALUES (96,11,24,4,32,'1',12,1);
INSERT INTO "PADA" VALUES (97,11,25,1,33,'',1,3);
INSERT INTO "PADA" VALUES (98,11,25,2,33,'1',2,1);
INSERT INTO "PADA" VALUES (99,11,25,3,33,'',3,3);
INSERT INTO "PADA" VALUES (100,12,25,4,34,'1',4,1);
INSERT INTO "PADA" VALUES (101,12,26,1,34,'',5,3);
INSERT INTO "PADA" VALUES (102,12,26,2,34,'1',6,1);
INSERT INTO "PADA" VALUES (103,12,26,3,35,'7',7,3);
INSERT INTO "PADA" VALUES (104,12,26,4,35,'',8,3);
INSERT INTO "PADA" VALUES (105,12,27,1,35,'',9,3);
INSERT INTO "PADA" VALUES (106,12,27,2,36,'4',10,5);
INSERT INTO "PADA" VALUES (107,12,27,3,36,'4',11,5);
INSERT INTO "PADA" VALUES (108,12,27,4,36,'4,2,3,8',12,2);
INSERT INTO "PLANET" VALUES (1,'SUN');
INSERT INTO "PLANET" VALUES (2,'MOON');
INSERT INTO "PLANET" VALUES (3,'MARS');
INSERT INTO "PLANET" VALUES (4,'MERCURY');
INSERT INTO "PLANET" VALUES (5,'JUPITER');
INSERT INTO "PLANET" VALUES (6,'VENUS');
INSERT INTO "PLANET" VALUES (7,'SATURN');
INSERT INTO "PLANET" VALUES (8,'RAHUMEAN');
INSERT INTO "PLANET" VALUES (9,'KETUMEAN');
INSERT INTO "PLANET" VALUES (10,'RAHUTRUE');
INSERT INTO "PLANET" VALUES (11,'KETUTRUE');
INSERT INTO "PLANET_DESC" VALUES (1,1,'Sun','en');
INSERT INTO "PLANET_DESC" VALUES (2,1,'Сонце','uk');
INSERT INTO "PLANET_DESC" VALUES (3,1,'Słońce','pl');
INSERT INTO "PLANET_DESC" VALUES (4,1,'Солнце','ru');
INSERT INTO "PLANET_DESC" VALUES (5,2,'Moon','en');
INSERT INTO "PLANET_DESC" VALUES (6,2,'Місяць','uk');
INSERT INTO "PLANET_DESC" VALUES (7,2,'Księżyc','pl');
INSERT INTO "PLANET_DESC" VALUES (8,2,'Луна','ru');
INSERT INTO "PLANET_DESC" VALUES (9,3,'Mars','en');
INSERT INTO "PLANET_DESC" VALUES (10,3,'Марс','uk');
INSERT INTO "PLANET_DESC" VALUES (11,3,'Mars','pl');
INSERT INTO "PLANET_DESC" VALUES (12,3,'Марс','ru');
INSERT INTO "PLANET_DESC" VALUES (13,4,'Mercury','en');
INSERT INTO "PLANET_DESC" VALUES (14,4,'Меркурій','uk');
INSERT INTO "PLANET_DESC" VALUES (15,4,'Merkury','pl');
INSERT INTO "PLANET_DESC" VALUES (16,4,'Меркурий','ru');
INSERT INTO "PLANET_DESC" VALUES (17,5,'Jupiter','en');
INSERT INTO "PLANET_DESC" VALUES (18,5,'Юпітер','uk');
INSERT INTO "PLANET_DESC" VALUES (19,5,'Jowisz','pl');
INSERT INTO "PLANET_DESC" VALUES (20,5,'Юпитер','ru');
INSERT INTO "PLANET_DESC" VALUES (21,6,'Venus','en');
INSERT INTO "PLANET_DESC" VALUES (22,6,'Венера','uk');
INSERT INTO "PLANET_DESC" VALUES (23,6,'Wenus','pl');
INSERT INTO "PLANET_DESC" VALUES (24,6,'Венера','ru');
INSERT INTO "PLANET_DESC" VALUES (25,7,'Saturn','en');
INSERT INTO "PLANET_DESC" VALUES (26,7,'Сатурн','uk');
INSERT INTO "PLANET_DESC" VALUES (27,7,'Saturn','pl');
INSERT INTO "PLANET_DESC" VALUES (28,7,'Сатурн','ru');
INSERT INTO "PLANET_DESC" VALUES (29,8,'Rahu','en');
INSERT INTO "PLANET_DESC" VALUES (30,8,'Раху','uk');
INSERT INTO "PLANET_DESC" VALUES (31,8,'Rahu','pl');
INSERT INTO "PLANET_DESC" VALUES (32,8,'Раху','ru');
INSERT INTO "PLANET_DESC" VALUES (33,9,'Ketu','en');
INSERT INTO "PLANET_DESC" VALUES (34,9,'Кету','uk');
INSERT INTO "PLANET_DESC" VALUES (35,9,'Ketu','pl');
INSERT INTO "PLANET_DESC" VALUES (36,9,'Кету','ru');
INSERT INTO "PLANET_DESC" VALUES (37,10,'Rahu','en');
INSERT INTO "PLANET_DESC" VALUES (38,10,'Раху','uk');
INSERT INTO "PLANET_DESC" VALUES (39,10,'Rahu','pl');
INSERT INTO "PLANET_DESC" VALUES (40,10,'Раху','ru');
INSERT INTO "PLANET_DESC" VALUES (41,11,'Ketu','en');
INSERT INTO "PLANET_DESC" VALUES (42,11,'Кету','uk');
INSERT INTO "PLANET_DESC" VALUES (43,11,'Ketu','pl');
INSERT INTO "PLANET_DESC" VALUES (44,11,'Кету','ru');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (1,1,'Pushkara Navamsa','en');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (2,2,'Vish Navamsa','en');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (3,3,'Gandanta','en');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (4,4,'Sarpa Drekkana','en');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (5,5,'Pasha Drekkana','en');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (6,6,'Nigada Drekkana','en');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (7,7,'Ashtamamsa','en');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (8,8,'Vargottama','en');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (9,1,'Пушкара Навамша','ru');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (10,2,'Виш Навамша','ru');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (11,3,'Ганданта','ru');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (12,4,'Сарпа Дреккана','ru');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (13,5,'Паша Дреккана','ru');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (14,6,'Нигада Дреккана','ru');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (15,7,'Аштамамша','ru');
INSERT INTO "SPECIALNAVAMSHA_DESC" VALUES (16,8,'Варготтама','ru');
INSERT INTO "SYSTEMFONT" VALUES (1,0,'Arial');
INSERT INTO "SYSTEMFONT" VALUES (2,0,'Book Antiqua');
INSERT INTO "SYSTEMFONT" VALUES (3,0,'Calibri');
INSERT INTO "SYSTEMFONT" VALUES (4,0,'Cambria');
INSERT INTO "SYSTEMFONT" VALUES (5,0,'Century');
INSERT INTO "SYSTEMFONT" VALUES (6,0,'Courier New');
INSERT INTO "SYSTEMFONT" VALUES (7,0,'Georgia');
INSERT INTO "SYSTEMFONT" VALUES (8,1,'Microsoft Sans Serif');
INSERT INTO "SYSTEMFONT" VALUES (9,0,'Monotype Corsiva');
INSERT INTO "SYSTEMFONT" VALUES (10,0,'Palatino Linotype');
INSERT INTO "SYSTEMFONT" VALUES (11,0,'Segoe UI');
INSERT INTO "SYSTEMFONT" VALUES (12,0,'Segoe Print');
INSERT INTO "SYSTEMFONT" VALUES (13,0,'Tahoma');
INSERT INTO "SYSTEMFONT" VALUES (14,0,'Times New Roman');
INSERT INTO "SYSTEMFONT" VALUES (15,0,'Yu Gothic UI Semilight');
INSERT INTO "TARABALA" VALUES (1,2);
INSERT INTO "TARABALA" VALUES (2,1);
INSERT INTO "TARABALA" VALUES (3,2);
INSERT INTO "TARABALA" VALUES (4,1);
INSERT INTO "TARABALA" VALUES (5,2);
INSERT INTO "TARABALA" VALUES (6,1);
INSERT INTO "TARABALA" VALUES (7,2);
INSERT INTO "TARABALA" VALUES (8,1);
INSERT INTO "TARABALA" VALUES (9,1);
INSERT INTO "TARABALA_DESC" VALUES (1,1,'Janma','Janma','Danger to the body, absence of risky actions, excellent state of routine duties','en');
INSERT INTO "TARABALA_DESC" VALUES (2,2,'Sampath','Sampath','Financial well-being, good food, family pleasures','en');
INSERT INTO "TARABALA_DESC" VALUES (3,3,'Vipat','Vipat','Dangers, losses and accidents, avoid important things','en');
INSERT INTO "TARABALA_DESC" VALUES (4,4,'Kshema','Kshema','Success in your efforts, follow your plans','en');
INSERT INTO "TARABALA_DESC" VALUES (5,5,'Pratyak','Pratyak','Obstacles to the path of your aspirations, postpone important matters','en');
INSERT INTO "TARABALA_DESC" VALUES (6,6,'Sadhana','Sadhana','Realizing your desires, assign important things to this time','en');
INSERT INTO "TARABALA_DESC" VALUES (7,7,'Naidhana','Naidhana','Losses, accidents, do not expect immediate results','en');
INSERT INTO "TARABALA_DESC" VALUES (8,8,'Mitra','Mitra','Promotes the implementation of ordinary cases','en');
INSERT INTO "TARABALA_DESC" VALUES (9,9,'Parama Mitra','P.Mitra','Very friendly to achieve the desired goal','en');
INSERT INTO "TARABALA_DESC" VALUES (10,1,'Джанма','Джанма','Опасность для тела, отсутствие рискованных действий, отличное состояние рутинных обязанностей','ru');
INSERT INTO "TARABALA_DESC" VALUES (11,2,'Сампат','Сампат','Финансовое благополучие, хорошая пища, семейные удовольствия','ru');
INSERT INTO "TARABALA_DESC" VALUES (12,3,'Випат','Випат','Опасность, потери и несчастные случаи, избегайте важных дел','ru');
INSERT INTO "TARABALA_DESC" VALUES (13,4,'Кшема','Кшема','Успех в ваших усилиях, следуйте вашим планам','ru');
INSERT INTO "TARABALA_DESC" VALUES (14,5,'Пратьяк','Пратьяк','Препятствия на пути ваших стремлений, отложите важные дела','ru');
INSERT INTO "TARABALA_DESC" VALUES (15,6,'Садхана','Садхана','Осуществление ваших желаний, назначайте на это время важные дела','ru');
INSERT INTO "TARABALA_DESC" VALUES (16,7,'Наидхана','Наидхана','Потери, несчастные случаи, не ожидайте немедленных результатов','ru');
INSERT INTO "TARABALA_DESC" VALUES (17,8,'Митра','Митра','Содействует выполнению обычных дел','ru');
INSERT INTO "TARABALA_DESC" VALUES (18,9,'Парама Митра','П.Митра','Очень дружественна достижению желаемой цели','ru');
INSERT INTO "TITHI" VALUES (1,1);
INSERT INTO "TITHI" VALUES (2,1);
INSERT INTO "TITHI" VALUES (3,1);
INSERT INTO "TITHI" VALUES (4,2);
INSERT INTO "TITHI" VALUES (5,1);
INSERT INTO "TITHI" VALUES (6,1);
INSERT INTO "TITHI" VALUES (7,1);
INSERT INTO "TITHI" VALUES (8,1);
INSERT INTO "TITHI" VALUES (9,2);
INSERT INTO "TITHI" VALUES (10,1);
INSERT INTO "TITHI" VALUES (11,1);
INSERT INTO "TITHI" VALUES (12,1);
INSERT INTO "TITHI" VALUES (13,1);
INSERT INTO "TITHI" VALUES (14,2);
INSERT INTO "TITHI" VALUES (15,1);
INSERT INTO "TITHI" VALUES (16,1);
INSERT INTO "TITHI" VALUES (17,1);
INSERT INTO "TITHI" VALUES (18,1);
INSERT INTO "TITHI" VALUES (19,2);
INSERT INTO "TITHI" VALUES (20,1);
INSERT INTO "TITHI" VALUES (21,1);
INSERT INTO "TITHI" VALUES (22,1);
INSERT INTO "TITHI" VALUES (23,1);
INSERT INTO "TITHI" VALUES (24,2);
INSERT INTO "TITHI" VALUES (25,1);
INSERT INTO "TITHI" VALUES (26,1);
INSERT INTO "TITHI" VALUES (27,1);
INSERT INTO "TITHI" VALUES (28,1);
INSERT INTO "TITHI" VALUES (29,2);
INSERT INTO "TITHI" VALUES (30,2);
INSERT INTO "TITHI_DESC" VALUES (1,1,'Shukla Prathama','Prathama','Sun, Agni','Nanda (satisfaction)','Planning; meditation; training; travels; entry into office; holding festivals; marriage; work related to the house and land; hair cut.','','en');
INSERT INTO "TITHI_DESC" VALUES (2,2,'Shukla Dwitiya','Dwitiya','Moon, Ashwins, Bramha','Bhadra (wise man)','The beginning of the actions connected with representatives of authority; marriage; jewelery decoration; laying the foundation of the house, building; fulfillment of promises; entry into office; the beginning of new cases; treatment; travels; creation.','','en');
INSERT INTO "TITHI_DESC" VALUES (3,3,'Shukla Tritiya','Tritiya','Mars, Gauri','Jaya (victory)','The beginning of important matters; marriage; travels; treatment; training; the first lesson in music; building.','','en');
INSERT INTO "TITHI_DESC" VALUES (4,4,'Shukla Chaturthi','Chaturthi','Mercury, Ganesha','Rikta (empty)','Cleaning, getting rid of unnecessary things, cleaning (home, body, mind); disputes, fighting with enemies; use of weapons, fire; removal of obstacles.','An unfavorable day for important things.','en');
INSERT INTO "TITHI_DESC" VALUES (5,5,'Shukla Panchami','Panchami','Jupiter, Naaga','Purna (fullness)','A very favorable day for starting important business; marriage; treatment; travels; training; the acquisition of wealth and the gift of gifts.','Lending money.','en');
INSERT INTO "TITHI_DESC" VALUES (6,6,'Shukla Shashthi','Shashthi','Venus, Karttikeya','Nanda (pleasure)','Building; cases involving houses or lands; manufacturing and putting on ornaments; training; establishment of friendship. Marriage is moderately favorable.','Trips.','en');
INSERT INTO "TITHI_DESC" VALUES (7,7,'Shukla Saptami','Saptami','Saturn, Surya','Bhadra (wise man)','The beginning of the trip; actions concerning vehicles; marriage; dancing, music; decoration with fabric or ornament; physical exercises (training); treatment.','','en');
INSERT INTO "TITHI_DESC" VALUES (8,8,'Shukla Ashtami','Ashtami','Rahu, Shiva','Jaya (victory)','Physical activity, sports; cases involving houses or lands; building; study of arts; training; meditation; rituals; healing and restorative procedures.','Travel; important projects; making decisions.','en');
INSERT INTO "TITHI_DESC" VALUES (9,9,'Shukla Navami','Navami','Sun, Durga','Rikta (empty)','Disputes; competition; physical exercises (training); work with tools; risk; meditation; rituals; reading mantras.','An unfavorable day for important things. Do not start trips and travel.','en');
INSERT INTO "TITHI_DESC" VALUES (10,10,'Shukla Dasami','Dasami','Moon, Yama','Purna (fullness)','The beginning of important matters; wedding; start date; dressing jewelry and new clothes; travels; cases involving vehicles, houses and important people.','','en');
INSERT INTO "TITHI_DESC" VALUES (11,11,'Shukla Ekadasi','Ekadasi','Mars, Kubera, Vishvadeva','Nanda (pleasure)','Restriction in food, fasting; meditation; fulfillment of promises; marriage; disputes; physical exercises (training); building; spiritual holidays; the adornment of something; travel; training.','','en');
INSERT INTO "TITHI_DESC" VALUES (12,12,'Shukla Dvadasi','Dvadasi','Mercury, Vishnu','Bhadra (wise man)','Fulfillment of promises; religious ceremonies; charity; other favorable actions. Marriage is moderately favorable.','Travels; oil massage; entrance to the new house.','en');
INSERT INTO "TITHI_DESC" VALUES (13,13,'Shukla Trayodasi','Trayodasi','Jupiter, Kamadeva','Jaya (victory)','The beginning of important matters; dressing new clothes and ornaments; fight; fulfillment of promises; treatment; travel.','','en');
INSERT INTO "TITHI_DESC" VALUES (14,14,'Shukla Chaturdash','Chaturdash','Venus, Shiva, Rudra','Rikta (empty)','Meditations, reading of sacred books, mantras.','An unfavorable day for important things. Avoid travel and hair cutting.','en');
INSERT INTO "TITHI_DESC" VALUES (15,15,'Purnima (full moon)','Purnima','Saturn, Chandra (Soma)/Pitru','Purna (fullness)','The beginning of important matters; building; entry into office; Beginning of work; housework; physical activity; spiritual ceremonies; fast; meditation.','Travel; oil massage; surgical operations.','en');
INSERT INTO "TITHI_DESC" VALUES (16,16,'Krishna Prathama','Prathama','Sun, Agni','Nanda (pleasure)','The beginning of important matters; training; festivals; travels; entry into office; marriage; cases involving houses or lands; hair cut.','','en');
INSERT INTO "TITHI_DESC" VALUES (17,17,'Krishna Dwitiya','Dwitiya','Moon, Ashwins, Bramha','Bhadra (wise man)','The beginning of new affairs; the formation of new goals; actions related to the authorities; marriage; jewelery decoration; laying the foundation of the house, building; fulfillment of promises; entry into office; beginning of work; treatment.','','en');
INSERT INTO "TITHI_DESC" VALUES (18,18,'Krishna Tritiya','Tritiya','Mars, Gauri','Jaya (victory)','The beginning of important matters; marriage; travels; treatment; training; the first lesson in music; building.','','en');
INSERT INTO "TITHI_DESC" VALUES (19,19,'Krishna Chaturthi','Chaturthi','Mercury, Ganesha','Rikta (empty)','Cleaning, getting rid of unnecessary things, cleaning (home, body, mind); disputes, fighting with enemies; use of weapons, fire; removal of obstacles.','An unfavorable day for important things.','en');
INSERT INTO "TITHI_DESC" VALUES (20,20,'Krishna Panchami','Panchami','Jupiter, Naaga','Purna (fullness)','A very favorable day for starting important business; marriage; treatment; travels; training; the acquisition of wealth and the gift of gifts.','Lending money.','en');
INSERT INTO "TITHI_DESC" VALUES (21,21,'Krishna Shashthi','Shashthi','Venus, Karttikeya','Nanda (pleasure)','Building; cases involving houses or lands; manufacturing and putting on ornaments; training; establishment of friendship. Marriage is moderately favorable.','Trips.','en');
INSERT INTO "TITHI_DESC" VALUES (22,22,'Krishna Saptami','Saptami','Saturn, Surya','Bhadra (wise man)','The beginning of the trip; actions concerning vehicles; marriage; dancing, music; decoration with fabric or ornament; physical exercises (training); treatment.','','en');
INSERT INTO "TITHI_DESC" VALUES (23,23,'Krishna Ashtami','Ashtami','Rahu, Shiva','Jaya (victory)','Physical activity, sports; cases involving houses or lands; building; study of arts; training; meditation; rituals; healing and restorative procedures.','Travel; important projects; making decisions.','en');
INSERT INTO "TITHI_DESC" VALUES (24,24,'Krishna Navami','Navami','Sun, Durga','Rikta (empty)','Disputes; competition; physical exercises (training); work with tools; risk; meditation; rituals; reading mantras.','An unfavorable day for important things. Do not start trips and travel.','en');
INSERT INTO "TITHI_DESC" VALUES (25,25,'Krishna Dasami','Dasami','Moon, Yama','Purna (fullness)','The beginning of important matters; wedding; start date; dressing jewelry and new clothes; travels; cases involving vehicles, houses and important people; healing procedures; oil massage.','','en');
INSERT INTO "TITHI_DESC" VALUES (26,26,'Krishna Ekadasi','Ekadasi','Mars, Kubera, Vishvadeva','Nanda (pleasure)','Restriction in food, fasting; meditation; fulfillment of promises; marriage; disputes; physical exercises (training); building; spiritual holidays; the adornment of something; travel; training.','','en');
INSERT INTO "TITHI_DESC" VALUES (27,27,'Krishna Dvadasi','Dvadasi','Mercury, Vishnu','Bhadra (wise man)','Fulfillment of promises; religious ceremonies; charity; other favorable actions. Marriage is moderately favorable.','Travels; oil massage; entrance to the new house.','en');
INSERT INTO "TITHI_DESC" VALUES (28,28,'Krishna Trayodasi','Trayodasi','Jupiter, Kamadeva','Jaya (victory)','The beginning of important matters; dressing new clothes and ornaments; fight; fulfillment of promises; treatment; travel.','','en');
INSERT INTO "TITHI_DESC" VALUES (29,29,'Krishna Chaturdash','Chaturdash','Venus, Shiva, Rudra','Rikta (empty)','Meditations, reading of sacred books, mantras.','An unfavorable day for important things. Avoid travel and hair cutting.','en');
INSERT INTO "TITHI_DESC" VALUES (30,30,'Amavasya (new moon)','Amavasya','Rahu, Chandra (Soma)/Pitru','Purna (fullness)','Spiritual practices; meditation; religious rituals; affairs related to the worship of the dead ancestors; work with the clan, asceticism.','An unfavorable day for almost any business.','en');
INSERT INTO "TITHI_DESC" VALUES (31,1,'Шукла Пратипат','Пратипат','Солнце, Агни','Нанда (удовлетворение)','Планирование; медитации; обучение; путешествия; вхождение в должность; проведение фестивалей; брак; работы, связанные с домом и земельным участком; подстригание волос.','','ru');
INSERT INTO "TITHI_DESC" VALUES (32,2,'Шукла Двитья','Двитья','Луна, Ашвини Кумара, Брахма','Бхадра (мудрец)','Начало действий, связанных с представителями власти; брак; украшение драгоценными камнями; закладка фундамента дома, строительство; выполнение обещаний; вхождение в должность; начало новых дел; лечение; путешествия; творчество.','','ru');
INSERT INTO "TITHI_DESC" VALUES (33,3,'Шукла Тритья','Тритья','Марс, Гаури (одно из имен Парвати)','Джая (победа)','Начало важных дел; брак; путешествия; лечение; обучение; первый урок музыки; строительство.','','ru');
INSERT INTO "TITHI_DESC" VALUES (34,4,'Шукла Чатуртхи','Чатуртхи','Меркурий, Ганеша','Риктха (пустые руки)','Уборка, избавления от ненужных вещей, чистки (дома, тела, ума); споры, борьба с врагами; использование оружия, огня; устранение препятствий.','Неблагоприятный день для важных дел.','ru');
INSERT INTO "TITHI_DESC" VALUES (35,5,'Шукла Панчами','Панчами','Юпитер, Нага (бог всех змей)','Пурна (полнота)','Очень благоприятный день для начинания важных дел; брак; лечение; путешествия; обучение; приобретение богатства и дарение подарков.','Одалживание денег.','ru');
INSERT INTO "TITHI_DESC" VALUES (36,6,'Шукла Шашти','Шашти','Венера, Картикея','Нанда (удовлетворение)','Строительство; дела, связанные с домами или землями; изготовление и надевание украшений; обучение; заведение дружбы. Брак средне благоприятен.','Поездки.','ru');
INSERT INTO "TITHI_DESC" VALUES (37,7,'Шукла Саптами','Саптами','Сатурн, Сурья','Бхадра (мудрец)','Начало поездки; действия, касающиеся транспортных средств; брак; танцы, музыка; украшение тканью или орнаментом; физические упражнения (тренировки); лечение.','','ru');
INSERT INTO "TITHI_DESC" VALUES (38,8,'Шукла Аштами','Аштами','Раху, Шива','Джая (победа)','Физическая активность, занятие спортом; дела, связанные с домами или землями; строительство; изучение искусств; обучение; медитации; ритуалы; целебные и восстановительные процедуры.','Поездки; важные проекты; принятие решений.','ru');
INSERT INTO "TITHI_DESC" VALUES (39,9,'Шукла Навами','Навами','Солнце, Дурга','Риктха (пустые руки)','Соревновательные действия; споры; конкуренция; физические упражнения (тренировки); работа с инструментами; риск; медитации; ритуалы; чтение мантр.','Неблагоприятный день для важных дел. Не начинать поездки и путешествия.','ru');
INSERT INTO "TITHI_DESC" VALUES (40,10,'Шукла Дашами','Дашами','Луна, Яма','Пурна (полнота)','Начало важных дел; свадьба; начало обучения; одевание украшений и новых одежд; путешествия; дела, связанные с транспортными средствами, домами и важными людьми.','','ru');
INSERT INTO "TITHI_DESC" VALUES (41,11,'Шукла Экадаши','Экадаши','Марс, Кубера, Вишвадева','Нанда (удовлетворение)','Ограничение в еде, пост; медитации; выполнение обещаний; брак; споры; физические упражнения (тренировки); строительство; духовные праздники; украшение чего-либо; поездки; обучение.','','ru');
INSERT INTO "TITHI_DESC" VALUES (42,12,'Шукла Двадаши','Двадаши','Меркурий, Вишну','Бхадра (мудрец)','Выполнение обещаний; религиозные церемонии; благотворительность; другие благоприятные действия. Брак средне благоприятен.','Путешествия; масляный массаж; вход в новый дом.','ru');
INSERT INTO "TITHI_DESC" VALUES (43,13,'Шукла Трайодаши','Трайодаши','Юпитер, Камадева','Джая (победа)','Начало важных дел; одевание новой одежды и украшений; борьба; выполнение обещаний; лечение; поездки.','','ru');
INSERT INTO "TITHI_DESC" VALUES (44,14,'Шукла Чатурдаши','Чатурдаши','Венера, Шива, Рудра','Риктха (пустые руки)','Медитации, чтение священных книг, мантр.','Неблагоприятный день для важных дел. Избегать путешествий и подстригания волос.','ru');
INSERT INTO "TITHI_DESC" VALUES (45,15,'Пурнима (Полнолуние)','Пурнима','Сатурн, Чандра (Сома)/Питри','Пурна (полнота)','Начало важных дел; строительство; вхождение в должность; начало работы; домашние дела; физическая активность; духовные церемонии; пост; медитации.','Поездки; масляный массаж; хирургические операции.','ru');
INSERT INTO "TITHI_DESC" VALUES (46,16,'Кришна Пратипат','Пратипат','Солнце, Агни','Нанда (удовлетворение)','Начало важных дел; обучение; фестивали; путешествия; вхождение в должность; брак; дела, связанные с домами или землями; подстригание волос.','','ru');
INSERT INTO "TITHI_DESC" VALUES (47,17,'Кришна Двитья','Двитья','Луна, Ашвини Кумара, Брахма','Бхадра (мудрец)','Начало новых дел; формировании новых целей;  действия, связанные с представителями власти; брак; украшение драгоценными камнями; закладка фундамента дома, строительство; выполнение обещаний; вхождение в должность; начало работы; лечение;','','ru');
INSERT INTO "TITHI_DESC" VALUES (48,18,'Кришна Тритья','Тритья','Марс, Гаури (одно из имен Парвати)','Джая (победа)','Начало важных дел; брак; путешествия; лечение; обучение; первый урок музыки; строительство.','','ru');
INSERT INTO "TITHI_DESC" VALUES (49,19,'Кришна Чатуртхи','Чатуртхи','Меркурий, Ганеша','Риктха (пустые руки)','Уборка, избавления от ненужных вещей, чистки (дома, тела, ума); споры, борьба с врагами; использование оружия, огня; устранение препятствий.','Неблагоприятный день для важных дел.','ru');
INSERT INTO "TITHI_DESC" VALUES (50,20,'Кришна Панчами','Панчами','Юпитер, Нага (бог всех змей)','Пурна (полнота)','Очень благоприятный день для начинания важных дел; брак; лечение; путешествия; обучение; приобретение богатства и дарение подарков.','Одалживание денег.','ru');
INSERT INTO "TITHI_DESC" VALUES (51,21,'Кришна Шашти','Шашти','Венера, Картикея','Нанда (удовлетворение)','Строительство; дела, связанные с домами или землями; изготовление и надевание украшений; обучение; заведение дружбы. Брак средне благоприятен.','Поездки.','ru');
INSERT INTO "TITHI_DESC" VALUES (52,22,'Кришна Саптами','Саптами','Сатурн, Сурья','Бхадра (мудрец)','Начало поездки; действия, касающиеся транспортных средств; брак; танцы, музыка; украшение тканью или орнаментом; физические упражнения (тренировки); лечение.','','ru');
INSERT INTO "TITHI_DESC" VALUES (53,23,'Кришна Аштами','Аштами','Раху, Шива','Джая (победа)','Физическая активность, занятие спортом; дела, связанные с домами или землями; строительство; изучение искусств; обучение; медитации; ритуалы; целебные и восстановительные процедуры.','Поездки; важные проекты; принятие решений.','ru');
INSERT INTO "TITHI_DESC" VALUES (54,24,'Кришна Навами','Навами','Солнце, Дурга','Риктха (пустые руки)','Соревновательные действия; споры; конкуренция; физические упражнения (тренировки); работа с инструментами; риск; медитации; ритуалы; чтение мантр.','Неблагоприятный день для важных дел. Не начинать поездки и путешествия.','ru');
INSERT INTO "TITHI_DESC" VALUES (55,25,'Кришна Дашами','Дашами','Луна, Яма','Пурна (полнота)','Начало важных дел; свадьба; начало обучения; одевание украшений и новых одежд; путешествия; дела, связанные с транспортными средствами, домами и важными людьми; целебные процедуры; масляный массаж.','','ru');
INSERT INTO "TITHI_DESC" VALUES (56,26,'Кришна Экадаши','Экадаши','Марс, Кубера, Вишвадева','Нанда (удовлетворение)','Ограничение в еде, пост; медитации; выполнение обещаний; брак; споры; физические упражнения (тренировки); строительство; духовные праздники; украшение чего-либо; поездки; обучение.','','ru');
INSERT INTO "TITHI_DESC" VALUES (57,27,'Кришна Двадаши','Двадаши','Меркурий, Вишну','Бхадра (мудрец)','Выполнение обещаний; религиозные церемонии; благотворительность; другие благоприятные действия. Брак средне благоприятен.','Путешествия; масляный массаж; вход в новый дом.','ru');
INSERT INTO "TITHI_DESC" VALUES (58,28,'Кришна Трайодаши','Трайодаши','Юпитер, Камадева','Джая (победа)','Начало важных дел; одевание новой одежды и украшений; борьба; выполнение обещаний; лечение; поездки.','','ru');
INSERT INTO "TITHI_DESC" VALUES (59,29,'Кришна Чатурдаши','Чатурдаши','Венера, Шива, Рудра','Риктха (пустые руки)','Медитации, чтение священных книг, мантр.','Неблагоприятный день для важных дел. Избегать путешествий и подстригания волос.','ru');
INSERT INTO "TITHI_DESC" VALUES (60,30,'Амавасья (Новолуние)','Амавасья','Раху, Чандра (Сома)/Питри','Пурна (полнота)','Духовные практики; медитации; религиозные ритуалы; дела, связанные с почитанием умерших предков; работа с родом, аскезы.','Неблагоприятный день почти для любых дел.','ru');
INSERT INTO "TRANSIT" VALUES (1,1,1,2,'');
INSERT INTO "TRANSIT" VALUES (2,1,2,2,'');
INSERT INTO "TRANSIT" VALUES (3,1,3,1,'9');
INSERT INTO "TRANSIT" VALUES (4,1,4,2,'');
INSERT INTO "TRANSIT" VALUES (5,1,5,2,'');
INSERT INTO "TRANSIT" VALUES (6,1,6,1,'12');
INSERT INTO "TRANSIT" VALUES (7,1,7,2,'');
INSERT INTO "TRANSIT" VALUES (8,1,8,2,'');
INSERT INTO "TRANSIT" VALUES (9,1,9,2,'');
INSERT INTO "TRANSIT" VALUES (10,1,10,1,'4');
INSERT INTO "TRANSIT" VALUES (11,1,11,1,'5');
INSERT INTO "TRANSIT" VALUES (12,1,12,2,'');
INSERT INTO "TRANSIT" VALUES (13,2,1,1,'5');
INSERT INTO "TRANSIT" VALUES (14,2,2,2,'');
INSERT INTO "TRANSIT" VALUES (15,2,3,1,'9');
INSERT INTO "TRANSIT" VALUES (16,2,4,2,'');
INSERT INTO "TRANSIT" VALUES (17,2,5,2,'');
INSERT INTO "TRANSIT" VALUES (18,2,6,1,'12');
INSERT INTO "TRANSIT" VALUES (19,2,7,1,'2');
INSERT INTO "TRANSIT" VALUES (20,2,8,2,'');
INSERT INTO "TRANSIT" VALUES (21,2,9,2,'');
INSERT INTO "TRANSIT" VALUES (22,2,10,1,'4');
INSERT INTO "TRANSIT" VALUES (23,2,11,1,'8');
INSERT INTO "TRANSIT" VALUES (24,2,12,2,'');
INSERT INTO "TRANSIT" VALUES (25,3,1,2,'');
INSERT INTO "TRANSIT" VALUES (26,3,2,2,'');
INSERT INTO "TRANSIT" VALUES (27,3,3,1,'12');
INSERT INTO "TRANSIT" VALUES (28,3,4,2,'');
INSERT INTO "TRANSIT" VALUES (29,3,5,2,'');
INSERT INTO "TRANSIT" VALUES (30,3,6,1,'9');
INSERT INTO "TRANSIT" VALUES (31,3,7,2,'');
INSERT INTO "TRANSIT" VALUES (32,3,8,2,'');
INSERT INTO "TRANSIT" VALUES (33,3,9,2,'');
INSERT INTO "TRANSIT" VALUES (34,3,10,2,'');
INSERT INTO "TRANSIT" VALUES (35,3,11,1,'5');
INSERT INTO "TRANSIT" VALUES (36,3,12,2,'');
INSERT INTO "TRANSIT" VALUES (37,4,1,2,'');
INSERT INTO "TRANSIT" VALUES (38,4,2,1,'5');
INSERT INTO "TRANSIT" VALUES (39,4,3,2,'');
INSERT INTO "TRANSIT" VALUES (40,4,4,1,'3');
INSERT INTO "TRANSIT" VALUES (41,4,5,2,'');
INSERT INTO "TRANSIT" VALUES (42,4,6,1,'9');
INSERT INTO "TRANSIT" VALUES (43,4,7,2,'');
INSERT INTO "TRANSIT" VALUES (44,4,8,1,'1');
INSERT INTO "TRANSIT" VALUES (45,4,9,2,'');
INSERT INTO "TRANSIT" VALUES (46,4,10,1,'8');
INSERT INTO "TRANSIT" VALUES (47,4,11,1,'12');
INSERT INTO "TRANSIT" VALUES (48,4,12,2,'');
INSERT INTO "TRANSIT" VALUES (49,5,1,2,'');
INSERT INTO "TRANSIT" VALUES (50,5,2,1,'12');
INSERT INTO "TRANSIT" VALUES (51,5,3,2,'');
INSERT INTO "TRANSIT" VALUES (52,5,4,2,'');
INSERT INTO "TRANSIT" VALUES (53,5,5,1,'4');
INSERT INTO "TRANSIT" VALUES (54,5,6,2,'');
INSERT INTO "TRANSIT" VALUES (55,5,7,1,'3');
INSERT INTO "TRANSIT" VALUES (56,5,8,2,'');
INSERT INTO "TRANSIT" VALUES (57,5,9,1,'10');
INSERT INTO "TRANSIT" VALUES (58,5,10,2,'');
INSERT INTO "TRANSIT" VALUES (59,5,11,1,'8');
INSERT INTO "TRANSIT" VALUES (60,5,12,2,'');
INSERT INTO "TRANSIT" VALUES (61,6,1,1,'8');
INSERT INTO "TRANSIT" VALUES (62,6,2,1,'7');
INSERT INTO "TRANSIT" VALUES (63,6,3,1,'1');
INSERT INTO "TRANSIT" VALUES (64,6,4,1,'10');
INSERT INTO "TRANSIT" VALUES (65,6,5,1,'9');
INSERT INTO "TRANSIT" VALUES (66,6,6,2,'');
INSERT INTO "TRANSIT" VALUES (67,6,7,2,'');
INSERT INTO "TRANSIT" VALUES (68,6,8,1,'5');
INSERT INTO "TRANSIT" VALUES (69,6,9,1,'11');
INSERT INTO "TRANSIT" VALUES (70,6,10,2,'');
INSERT INTO "TRANSIT" VALUES (71,6,11,1,'3');
INSERT INTO "TRANSIT" VALUES (72,6,12,1,'6');
INSERT INTO "TRANSIT" VALUES (73,7,1,2,'');
INSERT INTO "TRANSIT" VALUES (74,7,2,2,'');
INSERT INTO "TRANSIT" VALUES (75,7,3,1,'12');
INSERT INTO "TRANSIT" VALUES (76,7,4,2,'');
INSERT INTO "TRANSIT" VALUES (77,7,5,2,'');
INSERT INTO "TRANSIT" VALUES (78,7,6,1,'9');
INSERT INTO "TRANSIT" VALUES (79,7,7,2,'');
INSERT INTO "TRANSIT" VALUES (80,7,8,2,'');
INSERT INTO "TRANSIT" VALUES (81,7,9,2,'');
INSERT INTO "TRANSIT" VALUES (82,7,10,2,'');
INSERT INTO "TRANSIT" VALUES (83,7,11,1,'5');
INSERT INTO "TRANSIT" VALUES (84,7,12,2,'');
INSERT INTO "TRANSIT" VALUES (85,8,1,2,'');
INSERT INTO "TRANSIT" VALUES (86,8,2,2,'');
INSERT INTO "TRANSIT" VALUES (87,8,3,1,'12');
INSERT INTO "TRANSIT" VALUES (88,8,4,2,'');
INSERT INTO "TRANSIT" VALUES (89,8,5,2,'');
INSERT INTO "TRANSIT" VALUES (90,8,6,1,'9');
INSERT INTO "TRANSIT" VALUES (91,8,7,2,'');
INSERT INTO "TRANSIT" VALUES (92,8,8,2,'');
INSERT INTO "TRANSIT" VALUES (93,8,9,2,'');
INSERT INTO "TRANSIT" VALUES (94,8,10,1,'');
INSERT INTO "TRANSIT" VALUES (95,8,11,1,'5');
INSERT INTO "TRANSIT" VALUES (96,8,12,2,'');
INSERT INTO "TRANSIT" VALUES (97,9,1,2,'');
INSERT INTO "TRANSIT" VALUES (98,9,2,2,'');
INSERT INTO "TRANSIT" VALUES (99,9,3,1,'12');
INSERT INTO "TRANSIT" VALUES (100,9,4,2,'');
INSERT INTO "TRANSIT" VALUES (101,9,5,2,'');
INSERT INTO "TRANSIT" VALUES (102,9,6,1,'9');
INSERT INTO "TRANSIT" VALUES (103,9,7,2,'');
INSERT INTO "TRANSIT" VALUES (104,9,8,2,'');
INSERT INTO "TRANSIT" VALUES (105,9,9,2,'');
INSERT INTO "TRANSIT" VALUES (106,9,10,1,'');
INSERT INTO "TRANSIT" VALUES (107,9,11,1,'5');
INSERT INTO "TRANSIT" VALUES (108,9,12,2,'');
INSERT INTO "TRANSIT_DESC" VALUES (1,1,'Reduced income, discomfort, possible relocations, fatigue and poor health.','en');
INSERT INTO "TRANSIT_DESC" VALUES (2,1,'Зменшення доходів, дискомфорт, можливі переїзди, втома та погане самопочуття.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (3,1,'Spadek dochodów, dyskomfort, możliwe przeprowadzki, zmęczenie i gorsze zdrowie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (4,1,'Сокращение доходов, дискомфорт, возможны перемещения с места на место, усталость и плохое здоровье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (5,2,'Rising expenses (possibly incomes too), eye issues, confusion.','en');
INSERT INTO "TRANSIT_DESC" VALUES (6,2,'Зростання витрат (можливо, й доходів), проблеми з очима, омани.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (7,2,'Wzrost wydatków (może także dochodów), problemy z oczami, złudzenia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (8,2,'Возрастание расходов, но, возможно, и доходов тоже, болезни глаз, заблуждения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (9,3,'Success, relief from illness, overcoming obstacles, more energy.','en');
INSERT INTO "TRANSIT_DESC" VALUES (10,3,'Успіх, полегшення від хвороб, подолання перешкод, більше енергії.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (11,3,'Sukces, ulga od chorób, pokonywanie przeszkód, więcej energii.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (12,3,'Успех, свобода от болезней, преодоление преград, возрастание энергии','ru');
INSERT INTO "TRANSIT_DESC" VALUES (13,4,'Relationship issues, loss of reputation, general malaise, stomach problems.','en');
INSERT INTO "TRANSIT_DESC" VALUES (14,4,'Проблеми у стосунках, втрата репутації, загальне нездужання, проблеми зі шлунком.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (15,4,'Problemy w relacjach, utrata reputacji, złe samopoczucie, kłopoty żołądkowe.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (16,4,'Проблемы во взаимоотношениях, бесчестие, общее недомогание, в частности, с желудком','ru');
INSERT INTO "TRANSIT_DESC" VALUES (17,5,'Sadness, confrontation, poor judgment, physical discomfort.','en');
INSERT INTO "TRANSIT_DESC" VALUES (18,5,'Смуток, протистояння, слабке судження, фізичний дискомфорт.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (19,5,'Smutek, konfrontacje, słaby osąd, fizyczny dyskomfort.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (20,5,'Печаль, противостояние, плохое суждение, телесное недомогание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (21,6,'Victory over rivals, joy, good health or recovery.','en');
INSERT INTO "TRANSIT_DESC" VALUES (22,6,'Перемога над суперниками, радість, добре здоров’я або одужання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (23,6,'Zwycięstwo nad przeciwnikami, radość, dobre zdrowie lub powrót do zdrowia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (24,6,'Победа над врагами, радость, хорошее здоровье или выздоровление','ru');
INSERT INTO "TRANSIT_DESC" VALUES (25,7,'Travel and moving around; issues at place of stay; poor health.','en');
INSERT INTO "TRANSIT_DESC" VALUES (26,7,'Подорожі й мандри; проблеми за місцем перебування; слабке здоров’я.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (27,7,'Podróże i wędrówki; kłopoty w miejscu pobytu; słabe zdrowie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (28,7,'Путешествия, странствия, проблемы в месте своего пребывания, плохое здоровье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (29,8,'Defeat, relationship troubles, separation, humiliation.','en');
INSERT INTO "TRANSIT_DESC" VALUES (30,8,'Поразка, труднощі у стосунках, розлука, приниження.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (31,8,'Porażka, problemy w relacjach, rozstanie, upokorzenie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (32,8,'Поражение, проблемы во взаимоотношениях, разлука, унижение','ru');
INSERT INTO "TRANSIT_DESC" VALUES (33,9,'Accidents, stomach issues, feverish overthinking, career setback.','en');
INSERT INTO "TRANSIT_DESC" VALUES (34,9,'Нещасні випадки, проблеми зі шлунком, гарячкове перенапруження розуму, спад у кар’єрі.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (35,9,'Wypadki, problemy żołądkowe, gorączkowe myślenie, regres w karierze.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (36,9,'Несчастные случаи, проблемы с желудком, лихорадочная умственная деятельность, спад в карьере','ru');
INSERT INTO "TRANSIT_DESC" VALUES (37,10,'Honors, plans come to life, public recognition.','en');
INSERT INTO "TRANSIT_DESC" VALUES (38,10,'Почесті, втілення задумів, суспільне визнання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (39,10,'Honory, realizacja planów, uznanie społeczne.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (40,10,'Много почестей, реализация задуманного, признание обществом','ru');
INSERT INTO "TRANSIT_DESC" VALUES (41,11,'Higher income, respect, health and prosperity.','en');
INSERT INTO "TRANSIT_DESC" VALUES (42,11,'Вищі доходи, повага, здоров’я та добробут.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (43,11,'Wyższe dochody, szacunek, zdrowie i pomyślność.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (44,11,'Рост доходов, уважение, здоровье и процветание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (45,12,'Expenses, losses, humiliation, release.','en');
INSERT INTO "TRANSIT_DESC" VALUES (46,12,'Витрати, втрати, приниження, звільнення.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (47,12,'Wydatki, straty, upokorzenie, uwolnienie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (48,12,'Расходы, потери, унижение, освобождение','ru');
INSERT INTO "TRANSIT_DESC" VALUES (49,13,'Good for pleasures—food, comfort, clothing; acquisitions and happiness.','en');
INSERT INTO "TRANSIT_DESC" VALUES (50,13,'Добре для задоволень — їжа, комфорт і одяг; придбання та радість.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (51,13,'Dobre na przyjemności — jedzenie, wygodę i odzież; zakupy i radość.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (52,13,'Хорошо для удовольствий – пища, удобства и одежда, приобретения и счастье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (53,14,'Less respect and money, more obstacles, communication issues.','en');
INSERT INTO "TRANSIT_DESC" VALUES (54,14,'Менше поваги й грошей, більше перешкод, проблеми в спілкуванні.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (55,14,'Mniej szacunku i pieniędzy, więcej przeszkód, trudności w komunikacji.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (56,14,'Меньше уважения, денег, увеличение преград, проблемы в общении','ru');
INSERT INTO "TRANSIT_DESC" VALUES (57,15,'Home comfort and available funds.','en');
INSERT INTO "TRANSIT_DESC" VALUES (58,15,'Домашнє щастя та доступні кошти.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (59,15,'Domowe szczęście i dostępne środki.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (60,15,'Домашнее счастье и доступные денежные средства','ru');
INSERT INTO "TRANSIT_DESC" VALUES (61,16,'Loss of trust in others, mental imbalance, poor health, emotional breakdowns.','en');
INSERT INTO "TRANSIT_DESC" VALUES (62,16,'Втрата довіри до інших, дисбаланс розуму, слабке здоров’я, емоційні зриви.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (63,16,'Utrata zaufania do innych, brak równowagi psychicznej, gorsze zdrowie, załamania emocjonalne.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (64,16,'Утрата веры в других, недостаток умственного равновесия, здоровья, эмоциональные срывы','ru');
INSERT INTO "TRANSIT_DESC" VALUES (65,17,'Upheaval and disappointment; business setbacks; poor health; impaired judgment.','en');
INSERT INTO "TRANSIT_DESC" VALUES (66,17,'Розлади й розчарування; невдачі в бізнесі; слабке здоров’я; ослаблене судження.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (67,17,'Zaburzenia i rozczarowania; niepowodzenia w biznesie; słabsze zdrowie; gorszy osąd.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (68,17,'Расстройства, разочарование, неудачи в бизнесе, слабое здоровье, плохая способность суждения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (69,18,'Happiness, rising income, victory over rivals, good health.','en');
INSERT INTO "TRANSIT_DESC" VALUES (70,18,'Радість, зростання доходів, перемога над опонентами, добре здоров’я.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (71,18,'Radość, wzrost dochodów, pokonanie rywali, dobre zdrowie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (72,18,'Счастье, рост доходов, победа над врагами, хорошее здоровье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (73,19,'Public recognition, unexpected income, friendship; good for relationships.','en');
INSERT INTO "TRANSIT_DESC" VALUES (74,19,'Визнання суспільством, неочікувані доходи, дружба; добре для стосунків.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (75,19,'Uznanie społeczne, niespodziewane wpływy, przyjaźnie; dobre dla relacji.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (76,19,'Признание обществом, неожиданные доходы, дружба, хорошо для отношений','ru');
INSERT INTO "TRANSIT_DESC" VALUES (77,20,'Troubles, risk of detention, difficulties and sorrow, health issues.','en');
INSERT INTO "TRANSIT_DESC" VALUES (78,20,'Біди, ризик затримання, труднощі й смуток, проблеми зі здоров’ям.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (79,20,'Kłopoty, ryzyko zatrzymania, trudności i smutek, problemy zdrowotne.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (80,20,'Беды, арест, трудности и печали, проблемы со здоровьем','ru');
INSERT INTO "TRANSIT_DESC" VALUES (81,21,'Fear, difficulties and sorrow; isolation; health issues.','en');
INSERT INTO "TRANSIT_DESC" VALUES (82,21,'Страх, труднощі та смуток; ізоляція; проблеми зі здоров’ям.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (83,21,'Strach, trudności i smutek; izolacja; problemy zdrowotne.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (84,21,'Страх, трудности и печали, изоляция и проблемы со здоровьем','ru');
INSERT INTO "TRANSIT_DESC" VALUES (85,22,'Well-being, goals achieved, career milestones, favor from authorities.','en');
INSERT INTO "TRANSIT_DESC" VALUES (86,22,'Добробут, досягнення цілей, кар’єрні здобутки, прихильність владних осіб.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (87,22,'Dobrobyt, realizacja celów, osiągnięcia zawodowe, przychylność władz.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (88,22,'Благополучие, достижение целей, карьерные свершения, благосклонность власть имущих','ru');
INSERT INTO "TRANSIT_DESC" VALUES (89,23,'Prosperity, new friends and solid income; happiness.','en');
INSERT INTO "TRANSIT_DESC" VALUES (90,23,'Процвітання, нові друзі й добрий дохід; радість.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (91,23,'Dobrobyt, nowi przyjaciele i dobry dochód; radość.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (92,23,'Процветание, новые друзья и хороший доход, счастье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (93,24,'Possible injuries, higher expenses, disagreements.','en');
INSERT INTO "TRANSIT_DESC" VALUES (94,24,'Можливі поранення, зростання витрат, непорозуміння.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (95,24,'Możliwe urazy, większe wydatki, nieporozumienia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (96,24,'Возможны ранения, увеличение расходов, разногласия','ru');
INSERT INTO "TRANSIT_DESC" VALUES (97,25,'Good position, new job, gains and achievements.','en');
INSERT INTO "TRANSIT_DESC" VALUES (98,25,'Гарна позиція, нова робота, прибутки та досягнення.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (99,25,'Dobra pozycja, nowa praca, zyski i osiągnięcia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (100,25,'Хорошее положение, новая работа, прибыли и достижения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (101,26,'Disappointment, losses, illness, hardships.','en');
INSERT INTO "TRANSIT_DESC" VALUES (102,26,'Розчарування, втрати, хвороба, труднощі.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (103,26,'Rozczarowanie, straty, choroba i trudności.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (104,26,'Разочарование, потери, болезнь, страдания','ru');
INSERT INTO "TRANSIT_DESC" VALUES (105,27,'Respect, recognition, rising income, good health.','en');
INSERT INTO "TRANSIT_DESC" VALUES (106,27,'Повага, визнання, зростання доходів, добре здоров’я.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (107,27,'Szacunek, uznanie, wzrost dochodów, dobre zdrowie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (108,27,'Уважение, признание, увеличение доходов, хорошее здоровье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (109,28,'Troubles, fear, failures, illness.','en');
INSERT INTO "TRANSIT_DESC" VALUES (110,28,'Проблеми, страх, невдачі, хвороби.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (111,28,'Kłopoty, strach, porażki, choroby.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (112,28,'Проблемы, страх, неудачи, болезни','ru');
INSERT INTO "TRANSIT_DESC" VALUES (113,29,'Respect, income, success and good relationships.','en');
INSERT INTO "TRANSIT_DESC" VALUES (114,29,'Повага, доходи, успіх і добрі стосунки.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (115,29,'Szacunek, dochody, sukces i dobre relacje.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (116,29,'Уважение, доходы, успех и хорошие взаимоотношения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (117,30,'Higher expenses, conflicts, health problems.','en');
INSERT INTO "TRANSIT_DESC" VALUES (118,30,'Зростання витрат, конфлікти, проблеми зі здоров’ям.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (119,30,'Większe wydatki, konflikty, problemy zdrowotne.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (120,30,'Увеличение расходов, конфликты, проблемы со здоровьем','ru');
INSERT INTO "TRANSIT_DESC" VALUES (121,31,'Conflict with partner, fatigue, increasing health issues (eyes, digestion).','en');
INSERT INTO "TRANSIT_DESC" VALUES (122,31,'Конфлікт із партнером, втома, більше проблем зі здоров’ям (очі, травлення).','uk');
INSERT INTO "TRANSIT_DESC" VALUES (123,31,'Konflikt z partnerem, zmęczenie, nasilone problemy zdrowotne (oczy, trawienie).','pl');
INSERT INTO "TRANSIT_DESC" VALUES (124,31,'Конфликт с партнёром, усталость, увеличение проблем со здоровьем – такие как болезни глаз и пищеварения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (125,32,'Health issues (bleeding, anemia), decline in well-being and trust, accidents, injuries, humiliation.','en');
INSERT INTO "TRANSIT_DESC" VALUES (126,32,'Проблеми зі здоров’ям (кровотечі, анемія), спад добробуту й довіри, аварії, поранення, приниження.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (127,32,'Problemy zdrowotne (krwawienia, anemia), spadek dobrobytu i zaufania, wypadki, urazy, upokorzenie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (128,32,'Увеличение проблем со здоровьем – такие как кровотечения и анемия, снижение благополучия и доверия со стороны других, аварии, ранения, унижение','ru');
INSERT INTO "TRANSIT_DESC" VALUES (129,33,'Loss of reputation, unexpected expenses, weaker health, defeat.','en');
INSERT INTO "TRANSIT_DESC" VALUES (130,33,'Втрата репутації, несподівані витрати, ослаблення здоров’я, поразка.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (131,33,'Utrata reputacji, nieoczekiwane wydatki, osłabienie zdrowia, porażka.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (132,33,'Упадок репутации, непредвиденные расходы, ослабление здоровья, поражение','ru');
INSERT INTO "TRANSIT_DESC" VALUES (133,34,'Windfall gains, success in disputes, but possible distress.','en');
INSERT INTO "TRANSIT_DESC" VALUES (134,34,'Неочікувані гроші, успіх у суперечках, але можливі прикрощі.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (135,34,'Nagłe pieniądze, sukces w sporach, ale możliwe zmartwienia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (136,34,'Шальные деньги, победа в спорах, но возможны огорчения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (137,35,'Growing respect, good reputation, gaining property, new friends.','en');
INSERT INTO "TRANSIT_DESC" VALUES (138,35,'Зростання поваги, добра репутація, набуття майна, нові друзі.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (139,35,'Większy szacunek, dobra reputacja, zdobycie majątku, nowi przyjaciele.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (140,35,'Рост уважения, хорошая репутация, обретение собственности, обретение новых друзей','ru');
INSERT INTO "TRANSIT_DESC" VALUES (141,36,'Unexpected expenses, conflicts with spouse, health issues (eyes, Pitta imbalance).','en');
INSERT INTO "TRANSIT_DESC" VALUES (142,36,'Несподівані витрати, конфлікти з дружиною/чоловіком, проблеми зі здоров’ям (очі, дисбаланс Пітти).','uk');
INSERT INTO "TRANSIT_DESC" VALUES (143,36,'Nieoczekiwane wydatki, konflikty z partnerem, problemy zdrowotne (oczy, zaburzenia Pitty).','pl');
INSERT INTO "TRANSIT_DESC" VALUES (144,36,'Непредвиденные траты, ссоры с женой, проблемы со здоровьем – такие как болезни глаз и расстройства Питты','ru');
INSERT INTO "TRANSIT_DESC" VALUES (145,37,'Heavy workload, poor advice, deception, false associations, conflicts, trouble during travel.','en');
INSERT INTO "TRANSIT_DESC" VALUES (146,37,'Багато роботи, погані поради, обман, хибні зв’язки, конфлікти, труднощі в дорозі.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (147,37,'Dużo pracy, złe rady, oszustwo, fałszywe kontakty, konflikty, problemy w podróży.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (148,37,'Много работы, плохой совет, обман, ложные ассоциации, конфликты, беды во время путешествия','ru');
INSERT INTO "TRANSIT_DESC" VALUES (149,38,'Gaining knowledge, success and wealth, though some reputation issues may arise.','en');
INSERT INTO "TRANSIT_DESC" VALUES (150,38,'Одержання знань, успіх і багатство, але можливі проблеми з репутацією.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (151,38,'Zdobywanie wiedzy, sukces i bogactwo, lecz możliwe problemy z reputacją.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (152,38,'Обретение знаний, успех и богатство, но некоторые проблемы с репутацией','ru');
INSERT INTO "TRANSIT_DESC" VALUES (153,39,'New friends and gains, but fear of authorities and enemies; excessive travel.','en');
INSERT INTO "TRANSIT_DESC" VALUES (154,39,'Нові друзі та вигоди, але страх перед владою й ворогами; надмірні подорожі.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (155,39,'Nowi przyjaciele i korzyści, lecz strach przed władzą i wrogami; zbyt wiele podróży.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (156,39,'Новые друзья, выгоды, но страх перед властью и врагами, излишне много путешествий','ru');
INSERT INTO "TRANSIT_DESC" VALUES (157,40,'Family prosperity, income, and career growth.','en');
INSERT INTO "TRANSIT_DESC" VALUES (158,40,'Добробут родини, дохід і кар’єрне зростання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (159,40,'Dobrobyt rodziny, dochody i rozwój kariery.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (160,40,'Процветание родственников и семьи, доходы, карьерный рост','ru');
INSERT INTO "TRANSIT_DESC" VALUES (161,41,'Misunderstandings with partner and children; personal matters remain stable.','en');
INSERT INTO "TRANSIT_DESC" VALUES (162,41,'Непорозуміння з партнером і дітьми; особисті справи лишаються стабільними.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (163,41,'Nieporozumienia z partnerem i dziećmi; sprawy osobiste pozostają stabilne.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (164,41,'Непонимание в отношениях с партнёром и детьми, но личные дела в полном порядке','ru');
INSERT INTO "TRANSIT_DESC" VALUES (165,42,'Stability, recognition, quick success, popularity.','en');
INSERT INTO "TRANSIT_DESC" VALUES (166,42,'Стабільність, визнання, швидкий успіх, популярність.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (167,42,'Stabilność, uznanie, szybki sukces, popularność.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (168,42,'Стабильность, признание, быстрый успех, популярность','ru');
INSERT INTO "TRANSIT_DESC" VALUES (169,43,'Low energy, conflicts, heavy thoughts.','en');
INSERT INTO "TRANSIT_DESC" VALUES (170,43,'Мало енергії, конфлікти, важкі думки.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (171,43,'Mało energii, konflikty, ciężkie myśli.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (172,43,'Мало энергии, конфликты и тяжеловесные мысли','ru');
INSERT INTO "TRANSIT_DESC" VALUES (173,44,'Relationship problems, conflict, illness, depression.','en');
INSERT INTO "TRANSIT_DESC" VALUES (174,44,'Проблеми у стосунках, конфлікт, хвороби, депресія.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (175,44,'Problemy w relacjach, konflikt, choroby, depresja.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (176,44,'Проблемы во взаимоотношениях, конфликт, болезни, депрессия','ru');
INSERT INTO "TRANSIT_DESC" VALUES (177,45,'Higher income, travel, good health.','en');
INSERT INTO "TRANSIT_DESC" VALUES (178,45,'Вищий дохід, подорожі, добре здоров’я.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (179,45,'Wyższy dochód, podróże, dobre zdrowie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (180,45,'Повышение доходов, путешествия, хорошее здоровье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (181,46,'Good decisions, rising status, recognition.','en');
INSERT INTO "TRANSIT_DESC" VALUES (182,46,'Правильні рішення, зростання статусу, визнання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (183,46,'Dobre decyzje, wzrost statusu, uznanie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (184,46,'Правильные решения, рост статуса, признание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (185,47,'Losses, illness, delays, obstacles.','en');
INSERT INTO "TRANSIT_DESC" VALUES (186,47,'Втрати, хвороби, затримки, перешкоди.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (187,47,'Straty, choroby, opóźnienia, przeszkody.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (188,47,'Потери, болезни, задержки, препятствия','ru');
INSERT INTO "TRANSIT_DESC" VALUES (189,48,'Respect, good reputation, help from friends, new opportunities.','en');
INSERT INTO "TRANSIT_DESC" VALUES (190,48,'Повага, добра репутація, допомога друзів, нові можливості.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (191,48,'Szacunek, dobra reputacja, pomoc przyjaciół, nowe możliwości.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (192,48,'Уважение, хорошая репутация, помощь друзей, новые возможности','ru');
INSERT INTO "TRANSIT_DESC" VALUES (193,49,'Losses and humiliation, illness, misfortunes.','en');
INSERT INTO "TRANSIT_DESC" VALUES (194,49,'Втрати й приниження, хвороба, нещастя.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (195,49,'Straty i upokorzenia, choroba, nieszczęścia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (196,49,'Потери и унижения, болезнь, несчастья','ru');
INSERT INTO "TRANSIT_DESC" VALUES (197,50,'Inspiration, success, happiness.','en');
INSERT INTO "TRANSIT_DESC" VALUES (198,50,'Натхнення, успіх, радість.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (199,50,'Inspiracja, sukces, radość.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (200,50,'Воодушевление, успех, счастье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (201,51,'Hostility, competition, tension.','en');
INSERT INTO "TRANSIT_DESC" VALUES (202,51,'Ворожість, суперництво, напруженість.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (203,51,'Wrogość, rywalizacja, napięcie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (204,51,'Враждебность, конкуренция, напряжённость','ru');
INSERT INTO "TRANSIT_DESC" VALUES (205,52,'Recognition, honors, professional success.','en');
INSERT INTO "TRANSIT_DESC" VALUES (206,52,'Визнання, пошана, професійний успіх.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (207,52,'Uznanie, zaszczyty, sukces zawodowy.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (208,52,'Признание, почести, успехи в профессии','ru');
INSERT INTO "TRANSIT_DESC" VALUES (209,53,'Happiness, creativity, gaining partner, children or property; development of good qualities.','en');
INSERT INTO "TRANSIT_DESC" VALUES (210,53,'Щастя, творчість, здобуття партнера, дітей чи майна; розвиток добрих якостей.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (211,53,'Szczęście, kreatywność, zdobycie partnera, dzieci lub majątku; rozwój dobrych cech.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (212,53,'Счастье, творчество, обретение партнёра, детей или собственности, развитие хороших качеств, добродетель','ru');
INSERT INTO "TRANSIT_DESC" VALUES (213,54,'Mental distress, sadness, friends turning into rivals.','en');
INSERT INTO "TRANSIT_DESC" VALUES (214,54,'Психічні розлади, смуток, друзі стають суперниками.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (215,54,'Zaburzenia psychiczne, smutek, przyjaciele stają się rywalami.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (216,54,'Поражение ума, печаль, друзья становятся врагами','ru');
INSERT INTO "TRANSIT_DESC" VALUES (217,55,'Happy relationships, pleasures, good income, pleasant communication, recognition.','en');
INSERT INTO "TRANSIT_DESC" VALUES (218,55,'Щасливі стосунки, радощі, добрий дохід, приємне спілкування, визнання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (219,55,'Udane relacje, przyjemności, dobry dochód, miła komunikacja, uznanie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (220,55,'Счастливые взаимоотношения, удовольствия, хороший доход, хорошее общение, признание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (221,56,'Dissatisfaction, obstacles, complications, illness.','en');
INSERT INTO "TRANSIT_DESC" VALUES (222,56,'Незадоволення, перешкоди, ускладнення, хвороба.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (223,56,'Niezadowolenie, przeszkody, komplikacje, choroba.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (224,56,'Неудовлетворённость, препятствия, осложнения, болезнь','ru');
INSERT INTO "TRANSIT_DESC" VALUES (225,57,'Growing influence, childbirth, success at work, wealth from unexpected sources, fame.','en');
INSERT INTO "TRANSIT_DESC" VALUES (226,57,'Зростання впливу, народження дітей, успіх у роботі, неочікуваний прибуток, слава.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (227,57,'Wzrost wpływów, narodziny dzieci, sukces w pracy, niespodziewany majątek, sława.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (228,57,'Рост влияния, рождение детей, успех в работе, богатство от неожиданного источника, слава, признание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (229,58,'Risk of losing position, loss of money and health.','en');
INSERT INTO "TRANSIT_DESC" VALUES (230,58,'Ризик втратити позицію, втрата грошей і здоров’я.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (231,58,'Ryzyko utraty pozycji, utrata pieniędzy i zdrowia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (232,58,'Риск потерять место и положение, утрата денег и здоровья','ru');
INSERT INTO "TRANSIT_DESC" VALUES (233,59,'Stability, success, status, restoration of previous position, recovery.','en');
INSERT INTO "TRANSIT_DESC" VALUES (234,59,'Стабільність, успіх, статус, відновлення позицій, одужання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (235,59,'Stabilność, sukces, status, odzyskanie pozycji, powrót do zdrowia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (236,59,'Стабильность, успех, статус, восстановление прежних позиций, выздоровление','ru');
INSERT INTO "TRANSIT_DESC" VALUES (237,60,'Consequences for honesty and virtue; increased sorrow, financial loss, exhaustion.','en');
INSERT INTO "TRANSIT_DESC" VALUES (238,60,'Наслідки за прямоту й чесноти; більше смутку, фінансові втрати, виснаження.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (239,60,'Konsekwencje za szczerość i uczciwość; więcej smutku, straty finansowe, wyczerpanie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (240,60,'Возможна расплата за прямоту и добродетель, увеличение горестей, финансовые потери, перенапряжение','ru');
INSERT INTO "TRANSIT_DESC" VALUES (241,61,'Pleasures, luxury, enjoyment, emotional comfort, ornaments.','en');
INSERT INTO "TRANSIT_DESC" VALUES (242,61,'Насолоди, розкіш, задоволення, емоційний комфорт, прикраси.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (243,61,'Przyjemności, luksus, zadowolenie, komfort emocjonalny, ozdoby.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (244,61,'Наслаждения, роскошь, удовольствия, эмоциональное удовлетворение, комфорт, украшения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (245,62,'Material gains, childbirth, romances, family happiness.','en');
INSERT INTO "TRANSIT_DESC" VALUES (246,62,'Матеріальні надбання, народження дітей, романи, сімейне щастя.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (247,62,'Zyski materialne, narodziny dzieci, romanse, rodzinne szczęście.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (248,62,'Материальные приобретения, рождение детей, романы, семейное счастье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (249,63,'Happiness, influence, wealth and respect, recognition, overcoming rivals’ schemes.','en');
INSERT INTO "TRANSIT_DESC" VALUES (250,63,'Щастя, вплив, багатство й повага, визнання, подолання інтриг ворогів.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (251,63,'Szczęście, wpływy, bogactwo i szacunek, uznanie, pokonanie intryg wrogów.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (252,63,'Счастье, влияние, богатство и уважение, известность, преодоление вражеских козней','ru');
INSERT INTO "TRANSIT_DESC" VALUES (253,64,'Overall prosperity, home comfort, strength and recognition.','en');
INSERT INTO "TRANSIT_DESC" VALUES (254,64,'Загальне процвітання, домашній комфорт, сила й визнання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (255,64,'Ogólny dobrobyt, domowy komfort, siła i uznanie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (256,64,'В целом процветание, домашнее счастье, сила и признание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (257,65,'Renewed friendships, rising reputation, influence, wealth and authority.','en');
INSERT INTO "TRANSIT_DESC" VALUES (258,65,'Відновлення дружби, зростання репутації, впливу, багатства та влади.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (259,65,'Odnawianie przyjaźni, wzrost reputacji, wpływów, bogactwa i władzy.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (260,65,'Возобновление контактов с друзьями, рост репутации, влияния, богатства и власти','ru');
INSERT INTO "TRANSIT_DESC" VALUES (261,66,'Fear of rivals, illness, humiliation—despite general prosperity.','en');
INSERT INTO "TRANSIT_DESC" VALUES (262,66,'Страх перед ворогами, хвороба, приниження — попри загальне процвітання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (263,66,'Strach przed wrogami, choroba, upokorzenie — mimo ogólnego dobrobytu.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (264,66,'Страх перед врагами, болезнь и унижение, впрочем, на фоне общего процветания','ru');
INSERT INTO "TRANSIT_DESC" VALUES (265,67,'Relationship difficulties, sorrow, humiliation, illness, danger.','en');
INSERT INTO "TRANSIT_DESC" VALUES (266,67,'Труднощі у стосунках, смуток, приниження, хвороби, небезпека.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (267,67,'Problemy w relacjach, smutek, upokorzenie, choroby, niebezpieczeństwo.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (268,67,'Трудности во взаимоотношениях, горести, унижение, болезни, опасности','ru');
INSERT INTO "TRANSIT_DESC" VALUES (269,68,'Happiness through partner, enjoyment, but possible complications.','en');
INSERT INTO "TRANSIT_DESC" VALUES (270,68,'Щастя завдяки партнеру, задоволення, але можливі ускладнення.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (271,68,'Szczęście dzięki partnerowi, przyjemność, lecz możliwe komplikacje.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (272,68,'Счастье благодаря партнёру, удовольствие, но возможны и осложнения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (273,69,'Buying a new home or luxury items; marriage if unmarried.','en');
INSERT INTO "TRANSIT_DESC" VALUES (274,69,'Купівля нового житла чи розкоші; шлюб, якщо людина ще не одружена.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (275,69,'Zakup nowego domu lub dóbr luksusowych; ślub, jeśli ktoś jest nieżonaty/niezamężna.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (276,69,'Покупка нового дома, предметов роскоши, женитьба, если ещё не женат','ru');
INSERT INTO "TRANSIT_DESC" VALUES (277,70,'Rise leading to conflicts, dishonor and disputes.','en');
INSERT INTO "TRANSIT_DESC" VALUES (278,70,'Піднесення, що призводить до конфліктів, безчестя й суперечок.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (279,70,'Wyniesienie prowadzące do konfliktów, hańby i sporów.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (280,70,'Возвышение, приводящее к разборкам, бесчестие и конфликты','ru');
INSERT INTO "TRANSIT_DESC" VALUES (281,71,'Higher income, gains through friends and relatives, comfort.','en');
INSERT INTO "TRANSIT_DESC" VALUES (282,71,'Вищі доходи, вигоди через друзів і родичів, комфорт.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (283,71,'Wyższe dochody, korzyści dzięki przyjaciołom i rodzinie, komfort.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (284,71,'Рост доходов, выгоды благодаря друзьям и родственникам, комфорт','ru');
INSERT INTO "TRANSIT_DESC" VALUES (285,72,'New friends, money, luxury items—but also some expenses.','en');
INSERT INTO "TRANSIT_DESC" VALUES (286,72,'Нові друзі, гроші, предмети розкоші — але й певні витрати.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (287,72,'Nowi przyjaciele, pieniądze, dobra luksusowe — ale również wydatki.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (288,72,'Обретение новых друзей, денег, предметов роскоши, но и некоторые затраты','ru');
INSERT INTO "TRANSIT_DESC" VALUES (289,73,'Danger, obstacles, foreign travel, financial loss, separation from family and friends, illness, misfortune.','en');
INSERT INTO "TRANSIT_DESC" VALUES (290,73,'Небезпека, перешкоди, закордонні подорожі, втрата грошей, розлука з родиною й друзями, хвороба, нещастя.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (291,73,'Niebezpieczeństwo, przeszkody, podróże zagraniczne, utrata pieniędzy, rozłąka z rodziną i przyjaciółmi, choroba, nieszczęście.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (292,73,'Опасность, препятствия, путешествия за границу, потеря денег, разлука с семьёй и друзьями, болезнь, несчастье','ru');
INSERT INTO "TRANSIT_DESC" VALUES (293,74,'Sadness, loss of comfort; possible gain of wealth but without enjoyment.','en');
INSERT INTO "TRANSIT_DESC" VALUES (294,74,'Смуток, втрата комфорту; можливе надбання багатства, але без змоги насолодитися ним.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (295,74,'Smutek, utrata komfortu; możliwe bogactwo, ale bez możliwości cieszenia się nim.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (296,74,'Печаль, утрата комфорта, возможно обретение богатства, но без возможности насладиться им','ru');
INSERT INTO "TRANSIT_DESC" VALUES (297,75,'Increase in wealth, property and comforts; good health; overcoming obstacles and rivals’ schemes.','en');
INSERT INTO "TRANSIT_DESC" VALUES (298,75,'Зростання багатства, майна й зручностей; добре здоров’я; подолання перешкод та інтриг ворогів.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (299,75,'Wzrost bogactwa, majątku i wygód; dobre zdrowie; pokonanie przeszkód i intryg wrogów.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (300,75,'Рост богатства, увеличение собственности и жизненных удобств, хорошее здоровье, преодоление препятствий и вражеских козней','ru');
INSERT INTO "TRANSIT_DESC" VALUES (301,76,'Possible separation from friends and family; emotional and mental unrest.','en');
INSERT INTO "TRANSIT_DESC" VALUES (302,76,'Можлива розлука з друзями й родиною; емоційне та ментальне сум’яття.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (303,76,'Możliwa rozłąka z przyjaciółmi i rodziną; zamieszanie emocjonalne i mentalne.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (304,76,'Возможна разлука с друзьями и домашними, смятение ума и эмоций','ru');
INSERT INTO "TRANSIT_DESC" VALUES (305,77,'Separation from children, financial loss, failed speculations, misunderstandings and conflicts.','en');
INSERT INTO "TRANSIT_DESC" VALUES (306,77,'Розлука з дітьми, фінансові втрати, невдалі спекуляції, непорозуміння й сварки.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (307,77,'Rozłąka z dziećmi, straty finansowe, nieudane spekulacje, nieporozumienia i konflikty.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (308,77,'Разлука с детьми, потеря денег, неудачные спекуляции, непонимание и ссоры','ru');
INSERT INTO "TRANSIT_DESC" VALUES (309,78,'Overcoming enemies and illness; beneficial relationships; gaining property.','en');
INSERT INTO "TRANSIT_DESC" VALUES (310,78,'Подолання ворогів і хвороб; вигідні стосунки; набуття майна.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (311,78,'Pokonanie wrogów i chorób; korzystne relacje; zdobycie majątku.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (312,78,'Преодоление врагов и болезней, выгодные взаимоотношения, обретение собственности','ru');
INSERT INTO "TRANSIT_DESC" VALUES (313,79,'Separation from partner and children; confused thinking; aimless movement.','en');
INSERT INTO "TRANSIT_DESC" VALUES (314,79,'Розлука з партнером і дітьми; плутане мислення; безцільні метання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (315,79,'Rozłąka z partnerem i dziećmi; chaotyczne myślenie; bezcelowe działania.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (316,79,'Разлука с партнёром и детьми, ущербное мышление, бесцельные метания','ru');
INSERT INTO "TRANSIT_DESC" VALUES (317,80,'Confrontation, health problems, possible humiliation, mistakes, injuries, losses.','en');
INSERT INTO "TRANSIT_DESC" VALUES (318,80,'Протистояння, проблеми зі здоров’ям, можливе приниження, помилки, поранення, втрати.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (319,80,'Konfrontacje, problemy zdrowotne, możliwe upokorzenia, błędy, urazy, straty.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (320,80,'Противостояние, проблемы со здоровьем, возможное унижение, ошибочные действия, травмы, потери','ru');
INSERT INTO "TRANSIT_DESC" VALUES (321,81,'Lack of luck, financial losses, hostility, unexpected problems, obstacles in spiritual life.','en');
INSERT INTO "TRANSIT_DESC" VALUES (322,81,'Нестача вдачі, фінансові втрати, ворожість, несподівані проблеми, труднощі у духовному житті.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (323,81,'Brak szczęścia, straty finansowe, wrogość, niespodziewane problemy, trudności duchowe.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (324,81,'Недостаток удачи, финансовые потери, враждебность, неожиданные проблемы, трудности в духовной жизни','ru');
INSERT INTO "TRANSIT_DESC" VALUES (325,82,'Possible new job; loss of reputation, wealth and status.','en');
INSERT INTO "TRANSIT_DESC" VALUES (326,82,'Можлива нова робота; втрата репутації, багатства й статусу.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (327,82,'Możliwa nowa praca; utrata reputacji, majątku i statusu.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (328,82,'Возможность найти работу, но утрата репутации, богатства и статуса','ru');
INSERT INTO "TRANSIT_DESC" VALUES (329,83,'Gains in wealth and property; higher status, but possible mistakes.','en');
INSERT INTO "TRANSIT_DESC" VALUES (330,83,'Надбання багатства й майна; підвищення статусу, але можливі помилки.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (331,83,'Zyski w bogactwie i majątku; wzrost statusu, lecz możliwe błędy.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (332,83,'Обретение богатства и собственности, статуса и положения, но возможны ошибочные действия','ru');
INSERT INTO "TRANSIT_DESC" VALUES (333,84,'Expenses, sorrow, illness, humiliation and sadness.','en');
INSERT INTO "TRANSIT_DESC" VALUES (334,84,'Витрати, смуток, хвороби, приниження й печаль.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (335,84,'Wydatki, smutek, choroby, upokorzenia i przygnębienie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (336,84,'Траты, горести, болезни, унижение и печаль','ru');
INSERT INTO "TRANSIT_DESC" VALUES (337,85,'Illness and fear.','en');
INSERT INTO "TRANSIT_DESC" VALUES (338,85,'Хвороба й страх.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (339,85,'Choroba i strach.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (340,85,'Болезнь и страх','ru');
INSERT INTO "TRANSIT_DESC" VALUES (341,86,'Loss of wealth, conflicts and misunderstandings.','en');
INSERT INTO "TRANSIT_DESC" VALUES (342,86,'Втрата багатства, конфлікти й непорозуміння.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (343,86,'Utrata majątku, konflikty i nieporozumienia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (344,86,'Потеря богатства, конфликты и непонимание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (345,87,'Happiness and good news.','en');
INSERT INTO "TRANSIT_DESC" VALUES (346,87,'Щастя й добрі новини.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (347,87,'Szczęście i dobre wiadomości.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (348,87,'Счастье и хорошие новости','ru');
INSERT INTO "TRANSIT_DESC" VALUES (349,88,'Illness, danger and discouragement.','en');
INSERT INTO "TRANSIT_DESC" VALUES (350,88,'Хвороба, небезпека й пригніченість.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (351,88,'Choroba, niebezpieczeństwo i przygnębienie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (352,88,'Болезнь, опасность и уныние','ru');
INSERT INTO "TRANSIT_DESC" VALUES (353,89,'Financial losses and suffering.','en');
INSERT INTO "TRANSIT_DESC" VALUES (354,89,'Фінансові втрати й страждання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (355,89,'Straty finansowe i cierpienie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (356,89,'Финансовые потери и страдание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (357,90,'Pleasure and happy relationships.','en');
INSERT INTO "TRANSIT_DESC" VALUES (358,90,'Задоволення й щасливі стосунки.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (359,90,'Przyjemność i udane relacje.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (360,90,'Удовольствие и счастливые взаимоотношения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (361,91,'Losses and fear.','en');
INSERT INTO "TRANSIT_DESC" VALUES (362,91,'Втрати й страх.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (363,91,'Straty i strach.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (364,91,'Потери и страх','ru');
INSERT INTO "TRANSIT_DESC" VALUES (365,92,'Health or life danger.','en');
INSERT INTO "TRANSIT_DESC" VALUES (366,92,'Небезпека для здоров’я чи життя.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (367,92,'Zagrożenie zdrowia lub życia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (368,92,'Опасность здоровью или жизни','ru');
INSERT INTO "TRANSIT_DESC" VALUES (369,93,'Conflict, dark thoughts and losses.','en');
INSERT INTO "TRANSIT_DESC" VALUES (370,93,'Конфлікт, похмурі думки й втрати.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (371,93,'Konflikt, ponure myśli i straty.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (372,93,'Конфликт, угрюмые мысли и потери','ru');
INSERT INTO "TRANSIT_DESC" VALUES (373,94,'Hostility and obstacles.','en');
INSERT INTO "TRANSIT_DESC" VALUES (374,94,'Ворожість і перешкоди.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (375,94,'Wrogość i przeszkody.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (376,94,'Враждебность, преграды','ru');
INSERT INTO "TRANSIT_DESC" VALUES (377,95,'Happiness and big money.','en');
INSERT INTO "TRANSIT_DESC" VALUES (378,95,'Щастя й великі гроші.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (379,95,'Szczęście i duże pieniądze.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (380,95,'Счастье и большие деньги','ru');
INSERT INTO "TRANSIT_DESC" VALUES (381,96,'Expenses and dangers.','en');
INSERT INTO "TRANSIT_DESC" VALUES (382,96,'Витрати й небезпека.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (383,96,'Wydatki i zagrożenia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (384,96,'Траты и опасности','ru');
INSERT INTO "TRANSIT_DESC" VALUES (385,97,'Illness and fear.','en');
INSERT INTO "TRANSIT_DESC" VALUES (386,97,'Хвороба й страх.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (387,97,'Choroba i strach.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (388,97,'Болезнь и страх','ru');
INSERT INTO "TRANSIT_DESC" VALUES (389,98,'Loss of wealth, conflicts and misunderstandings.','en');
INSERT INTO "TRANSIT_DESC" VALUES (390,98,'Втрата багатства, конфлікти й непорозуміння.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (391,98,'Utrata majątku, konflikty i nieporozumienia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (392,98,'Потеря богатства, конфликты и непонимание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (393,99,'Happiness and good news.','en');
INSERT INTO "TRANSIT_DESC" VALUES (394,99,'Щастя й добрі новини.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (395,99,'Szczęście i dobre wiadomości.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (396,99,'Счастье и хорошие новости','ru');
INSERT INTO "TRANSIT_DESC" VALUES (397,100,'Illness, danger and discouragement.','en');
INSERT INTO "TRANSIT_DESC" VALUES (398,100,'Хвороба, небезпека й пригніченість.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (399,100,'Choroba, niebezpieczeństwo i przygnębienie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (400,100,'Болезнь, опасность и уныние','ru');
INSERT INTO "TRANSIT_DESC" VALUES (401,101,'Financial losses and suffering.','en');
INSERT INTO "TRANSIT_DESC" VALUES (402,101,'Фінансові втрати й страждання.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (403,101,'Straty finansowe i cierpienie.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (404,101,'Финансовые потери и страдание','ru');
INSERT INTO "TRANSIT_DESC" VALUES (405,102,'Pleasure and happy relationships.','en');
INSERT INTO "TRANSIT_DESC" VALUES (406,102,'Задоволення й щасливі стосунки.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (407,102,'Przyjemność i udane relacje.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (408,102,'Удовольствие и счастливые взаимоотношения','ru');
INSERT INTO "TRANSIT_DESC" VALUES (409,103,'Losses and fear.','en');
INSERT INTO "TRANSIT_DESC" VALUES (410,103,'Втрати й страх.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (411,103,'Straty i strach.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (412,103,'Потери и страх','ru');
INSERT INTO "TRANSIT_DESC" VALUES (413,104,'Health or life danger.','en');
INSERT INTO "TRANSIT_DESC" VALUES (414,104,'Небезпека для здоров’я чи життя.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (415,104,'Zagrożenie zdrowia lub życia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (416,104,'Опасность здоровью или жизни','ru');
INSERT INTO "TRANSIT_DESC" VALUES (417,105,'Conflict, dark thoughts and losses.','en');
INSERT INTO "TRANSIT_DESC" VALUES (418,105,'Конфлікт, похмурі думки й втрати.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (419,105,'Konflikt, ponure myśli i straty.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (420,105,'Конфликт, угрюмые мысли и потери','ru');
INSERT INTO "TRANSIT_DESC" VALUES (421,106,'Hostility and obstacles.','en');
INSERT INTO "TRANSIT_DESC" VALUES (422,106,'Ворожість і перешкоди.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (423,106,'Wrogość i przeszkody.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (424,106,'Враждебность, преграды','ru');
INSERT INTO "TRANSIT_DESC" VALUES (425,107,'Happiness and big money.','en');
INSERT INTO "TRANSIT_DESC" VALUES (426,107,'Щастя й великі гроші.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (427,107,'Szczęście i duże pieniądze.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (428,107,'Счастье и большие деньги','ru');
INSERT INTO "TRANSIT_DESC" VALUES (429,108,'Expenses and dangers.','en');
INSERT INTO "TRANSIT_DESC" VALUES (430,108,'Витрати й небезпека.','uk');
INSERT INTO "TRANSIT_DESC" VALUES (431,108,'Wydatki i zagrożenia.','pl');
INSERT INTO "TRANSIT_DESC" VALUES (432,108,'Траты и опасности','ru');
INSERT INTO "YOGA" VALUES (1,1,'DWIPUSHKAR');
INSERT INTO "YOGA" VALUES (2,1,'TRIPUSHKAR');
INSERT INTO "YOGA" VALUES (3,1,'AMRITASIDDHA');
INSERT INTO "YOGA" VALUES (4,1,'SARVARTHA');
INSERT INTO "YOGA" VALUES (5,1,'SIDDHA');
INSERT INTO "YOGA" VALUES (6,2,'MRITYU');
INSERT INTO "YOGA" VALUES (7,2,'ADHAM');
INSERT INTO "YOGA" VALUES (8,2,'YAMAGHATA');
INSERT INTO "YOGA" VALUES (9,2,'DAGDHA');
INSERT INTO "YOGA" VALUES (10,2,'UNFAVORABLE');
INSERT INTO "YOGA_DESC" VALUES (1,1,'Dwipushkar Yoga','DPY','Favorable','en');
INSERT INTO "YOGA_DESC" VALUES (2,2,'Tripushkar Yoga','TPY','Favorable','en');
INSERT INTO "YOGA_DESC" VALUES (3,3,'Amrita Siddha Yoga','ASY','Favorable','en');
INSERT INTO "YOGA_DESC" VALUES (4,4,'Sarvartha Siddha Yoga','SSY','Favorable','en');
INSERT INTO "YOGA_DESC" VALUES (5,5,'Siddha Yoga','SY','Favorable','en');
INSERT INTO "YOGA_DESC" VALUES (6,6,'Mrityu Yoga','MY','Unfavorable','en');
INSERT INTO "YOGA_DESC" VALUES (7,7,'Adham Yoga','AY','Unfavorable','en');
INSERT INTO "YOGA_DESC" VALUES (8,8,'Yamaghata Yoga','YY','Unfavorable','en');
INSERT INTO "YOGA_DESC" VALUES (9,9,'Dagdha Yoga','DY','Unfavorable','en');
INSERT INTO "YOGA_DESC" VALUES (10,10,'Unfavorable Yoga','UY','Unfavorable','en');
INSERT INTO "YOGA_DESC" VALUES (11,1,'Двипушкар Йога','ДПЙ','Благоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (12,2,'Трипушкар Йога','ТПЙ','Благоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (13,3,'Амрита Сиддха Йога','АСЙ','Благоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (14,4,'Сарвартха Сиддха Йога','ССЙ','Благоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (15,5,'Сиддха Йога','СЙ','Благоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (16,6,'Мритью Йога','МЙ','Неблагоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (17,7,'Адхам Йога','АЙ','Неблагоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (18,8,'Ямагхата Йога','ЯЙ','Неблагоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (19,9,'Дагдха Йога','ДЙ','Неблагоприятная','ru');
INSERT INTO "YOGA_DESC" VALUES (20,10,'Неблагоприятная Йога','НЙ','Неблагоприятная','ru');
INSERT INTO "ZODIAC" VALUES (1,'ARI');
INSERT INTO "ZODIAC" VALUES (2,'TAU');
INSERT INTO "ZODIAC" VALUES (3,'GEM');
INSERT INTO "ZODIAC" VALUES (4,'CNC');
INSERT INTO "ZODIAC" VALUES (5,'LEO');
INSERT INTO "ZODIAC" VALUES (6,'VIR');
INSERT INTO "ZODIAC" VALUES (7,'LIB');
INSERT INTO "ZODIAC" VALUES (8,'SCO');
INSERT INTO "ZODIAC" VALUES (9,'SGR');
INSERT INTO "ZODIAC" VALUES (10,'CAP');
INSERT INTO "ZODIAC" VALUES (11,'AQR');
INSERT INTO "ZODIAC" VALUES (12,'PSC');
INSERT INTO "ZODIAC_DESC" VALUES (1,1,'Aries','en');
INSERT INTO "ZODIAC_DESC" VALUES (2,1,'Овен','uk');
INSERT INTO "ZODIAC_DESC" VALUES (3,1,'Baran','pl');
INSERT INTO "ZODIAC_DESC" VALUES (4,1,'Овен','ru');
INSERT INTO "ZODIAC_DESC" VALUES (5,2,'Taurus','en');
INSERT INTO "ZODIAC_DESC" VALUES (6,2,'Телець','uk');
INSERT INTO "ZODIAC_DESC" VALUES (7,2,'Byk','pl');
INSERT INTO "ZODIAC_DESC" VALUES (8,2,'Телец','ru');
INSERT INTO "ZODIAC_DESC" VALUES (9,3,'Gemini','en');
INSERT INTO "ZODIAC_DESC" VALUES (10,3,'Близнюки','uk');
INSERT INTO "ZODIAC_DESC" VALUES (11,3,'Bliźnięta','pl');
INSERT INTO "ZODIAC_DESC" VALUES (12,3,'Близнецы','ru');
INSERT INTO "ZODIAC_DESC" VALUES (13,4,'Cancer','en');
INSERT INTO "ZODIAC_DESC" VALUES (14,4,'Рак','uk');
INSERT INTO "ZODIAC_DESC" VALUES (15,4,'Rak','pl');
INSERT INTO "ZODIAC_DESC" VALUES (16,4,'Рак','ru');
INSERT INTO "ZODIAC_DESC" VALUES (17,5,'Leo','en');
INSERT INTO "ZODIAC_DESC" VALUES (18,5,'Лев','uk');
INSERT INTO "ZODIAC_DESC" VALUES (19,5,'Lew','pl');
INSERT INTO "ZODIAC_DESC" VALUES (20,5,'Лев','ru');
INSERT INTO "ZODIAC_DESC" VALUES (21,6,'Virgo','en');
INSERT INTO "ZODIAC_DESC" VALUES (22,6,'Діва','uk');
INSERT INTO "ZODIAC_DESC" VALUES (23,6,'Panna','pl');
INSERT INTO "ZODIAC_DESC" VALUES (24,6,'Дева','ru');
INSERT INTO "ZODIAC_DESC" VALUES (25,7,'Libra','en');
INSERT INTO "ZODIAC_DESC" VALUES (26,7,'Терези','uk');
INSERT INTO "ZODIAC_DESC" VALUES (27,7,'Waga','pl');
INSERT INTO "ZODIAC_DESC" VALUES (28,7,'Весы','ru');
INSERT INTO "ZODIAC_DESC" VALUES (29,8,'Scorpio','en');
INSERT INTO "ZODIAC_DESC" VALUES (30,8,'Скорпіон','uk');
INSERT INTO "ZODIAC_DESC" VALUES (31,8,'Skorpion','pl');
INSERT INTO "ZODIAC_DESC" VALUES (32,8,'Скорпион','ru');
INSERT INTO "ZODIAC_DESC" VALUES (33,9,'Sagittarius','en');
INSERT INTO "ZODIAC_DESC" VALUES (34,9,'Стрілець','uk');
INSERT INTO "ZODIAC_DESC" VALUES (35,9,'Strzelec','pl');
INSERT INTO "ZODIAC_DESC" VALUES (36,9,'Стрелец','ru');
INSERT INTO "ZODIAC_DESC" VALUES (37,10,'Capricorn','en');
INSERT INTO "ZODIAC_DESC" VALUES (38,10,'Козоріг','uk');
INSERT INTO "ZODIAC_DESC" VALUES (39,10,'Koziorożec','pl');
INSERT INTO "ZODIAC_DESC" VALUES (40,10,'Козерог','ru');
INSERT INTO "ZODIAC_DESC" VALUES (41,11,'Aquarius','en');
INSERT INTO "ZODIAC_DESC" VALUES (42,11,'Водолій','uk');
INSERT INTO "ZODIAC_DESC" VALUES (43,11,'Wodnik','pl');
INSERT INTO "ZODIAC_DESC" VALUES (44,11,'Водолей','ru');
INSERT INTO "ZODIAC_DESC" VALUES (45,12,'Pisces','en');
INSERT INTO "ZODIAC_DESC" VALUES (46,12,'Риби','uk');
INSERT INTO "ZODIAC_DESC" VALUES (47,12,'Ryby','pl');
INSERT INTO "ZODIAC_DESC" VALUES (48,12,'Рыбы','ru');
COMMIT;
