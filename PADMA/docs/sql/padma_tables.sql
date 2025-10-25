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
	FOREIGN KEY("ZODIAKID") REFERENCES "ZODIAK"("ID")
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
	"ZODIAKID"	INTEGER,
	"DEGREE"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("PLANETID") REFERENCES "PLANET"("ID"),
	FOREIGN KEY("ZODIAKID") REFERENCES "ZODIAK"("ID")
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
	"UPRAVITEL"	TEXT,
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
	FOREIGN KEY("NITYAYOGAID") REFERENCES "NITYAYOGA_OLD"("ID")
);
CREATE TABLE IF NOT EXISTS "PADA" (
	"ID"	INTEGER,
	"ZODIAKID"	INTEGER,
	"NAKSHATRAID"	INTEGER,
	"PADANUMBER"	INTEGER,
	"DREKKANA"	INTEGER,
	"SPECIALNAVAMSHA"	TEXT,
	"NAVAMSHA"	INTEGER,
	"COLORID"	INTEGER,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID"),
	FOREIGN KEY("NAKSHATRAID") REFERENCES "NAKSHATRA"("ID"),
	FOREIGN KEY("ZODIAKID") REFERENCES "ZODIAK"("ID")
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
CREATE TABLE IF NOT EXISTS "TRANZIT" (
	"ID"	INTEGER,
	"PLANETID"	INTEGER,
	"DOM"	INTEGER,
	"COLORID"	INTEGER,
	"VEDHA"	TEXT,
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("COLORID") REFERENCES "COLOR"("ID"),
	FOREIGN KEY("PLANETID") REFERENCES "PLANET"("ID")
);
CREATE TABLE IF NOT EXISTS "TRANZIT_DESC" (
	"ID"	INTEGER,
	"TRANZITID"	INTEGER,
	"DESCRIPTION"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("TRANZITID") REFERENCES "TRANZIT"("ID")
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
CREATE TABLE IF NOT EXISTS "ZODIAK" (
	"ID"	INTEGER,
	"ZODIAKCODE"	VARCHAR(3),
	PRIMARY KEY("ID" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "ZODIAK_DESC" (
	"ID"	INTEGER,
	"ZODIAKID"	INTEGER,
	"NAME"	TEXT,
	"LANGUAGECODE"	VARCHAR(2),
	PRIMARY KEY("ID" AUTOINCREMENT),
	FOREIGN KEY("ZODIAKID") REFERENCES "ZODIAK"("ID")
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
INSERT INTO "APP_META" VALUES ('DB_VERSION','0.0.18');
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
INSERT INTO "ECLIPSE_DESC" VALUES (1,1,'Moon eclipse','en');
INSERT INTO "ECLIPSE_DESC" VALUES (2,2,'Sun eclipse','en');
INSERT INTO "ECLIPSE_DESC" VALUES (3,1,'Лунное затмение','ru');
INSERT INTO "ECLIPSE_DESC" VALUES (4,2,'Солнечное затмение','ru');
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
INSERT INTO "NAKSHATRA_DESC" VALUES (1,1,'Ashwini','Ashwini','Ketu, Ashwins','Fast, Light(Kshipra)','Nakshatra is favorable for action when you need a quick result. It enhances the speed of events, activities. Also, good for events and actions, during which we desire changes or actions that cause changes in the future.','Actions related to human health (treatment, preparation and taking of medicines, surgeries (plastic), procedures), cosmetology procedures, hair and nails cutting, sports, beginning of training, studying astrology and spiritual sciences, starting a business, opening a store, buying, sale (supply advertisements for sale), purchase or sale of vehicles, travel (commencement of travel), jewelry works, making and dressing jewelry, dressing new clothes, activities related to art, forming partnerships from legal actions, started construction, moving, planting plants (especially on the growing moon).','To complete something, for any long-term activity, for emotional events, conversations, marriage.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (2,2,'Bharani','Bharani','Venus, Yama','Fierce or Severe (Ugra)','Nakshatra is favorable for events or actions that require intensity, ferocity, recklessness. Gives the power to overcome problems with hardness, pressure. Nakshatra helps to overcome ordinary abilities and work beyond capacity.','Activities are cruel and difficult to perform, requiring overcoming of obstacles, competition, victory over the enemy, the destruction of something (divorce), the completion of some process, dismissal from work, working with arms, agricultural activities, working with fire, poisons, chemical substances, digging wells, pruning trees, cleaning procedures, starvation, spiritual practices, meditation, yoga.','The beginning of activity, the beginning of travel, purchase. Risk of poisoning, accidents, deceptions. Do not take money on bail.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (3,3,'Krittika','Krittika','Sun, Agni','Mixed (Gentle, Fearsome) (Mridu-Tikshna)','Nakshatra is favorable for events with little importance. You can engage in routine activities, daily duties, but do not start new important things.','Sharp, swift actions, competition, manifestation of courage, bravery, destruction of the enemy, contracts, meetings (business), heated discussions (disputes), public appearances, lawsuits, removal of something, cleaning procedures, throwing out, cleaning, getting rid of, get out, get divorced, get rid of bad habits, get a haircut, shave, epilation, work with metals, buy knives, devices where there are knives, work with weapons, activities related to heat (heating systems).','The beginning of new important affairs, communication, diplomacy, recreation, activities related to water (buying a boat, going on a sea voyage). You can not take or lend money.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (4,4,'Rohini','Rohini','Moon, Brahma','Fixed or Permanent (Dhruva)','Nakshatra is favorable for the beginning of events, actions for a long period, the creation of what we do not want to further change in the future. It is necessary to carry out activities designed for long-term results. Started at this time will bring good steady fruit.','Actions aimed at the development and expansion of something, marriage, the beginning of relationships, actions to improve health, long-term investments, the accumulation of wealth, the first dressing of jewelry, working with clothes, ornaments, decorations, jewelry, moving to a new house, building, laying foundation, installation, installation of something, engineering, agricultural work, planting seeds, buying/selling, travelling, taking oaths, vows.','Completion of something, destruction.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (5,5,'Mrigashira','Mrigashira','Mars, Chandra (Soma)','Soft, Gentle (Mridu)','Nakshatra is favorable for the new relations development, the beginning of new affairs. Good for any gentle, joyful, emotional, entertaining and comforting affairs, events.','Friendship institution, marriage union, child conception, treatment, construction, foundation laying, travel, change of residence and moving, performing ceremonial ceremonies, establishing icons or figurines of the deity, religious affairs, donations, reverence, starting learning, communication, advertising, selling, art, hair cutting, wearing new clothes.','Conflicts, the acceptance of serious decisions.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (6,6,'Ardra','Ardra','Rahu, Rudra','Fearsome, Sharp (Tikshna)','Nakshatra is favorable for active actions, meeting with the enemy, for obtaining the results of work at any cost, for actions related to pain, damage. Sometimes painful things (temporarily) like surgery can be useful in the future.','Commitment, offensive, meeting with the enemy, disputes, breaking contracts, destruction, getting rid of old things, habits, working with fire and poisons, working with weapons.','Any beginnings, marriage, travel, ceremony, relocation, shopping.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (7,7,'Punarvasu','Punarvasu','Jupiter, Aditi','Mobile, Temporary (Chara)','Nakshatra is favorable for events and actions, during which we desire changes or actions that will cause changes in the future, for cases involving changes or movement, for temporary, short-lived results. The events that took place in this nakshatra are repeated.','Actions aimed at the development and expansion of something, business, relationships, the beginning of treatment, medical procedures, the onset of fasting, fasting, diet, travel, the acquisition (repair) of vehicles, construction, foundation laying, the beginning of repair, hair and nails cutting, jewelry works, training.','Rough actions, conflicts, disputes, courts, take and lend money.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (8,8,'Pushya','Pushya','Saturn, Brhaspati','Fast, Light(Kshipra)','Nakshatra is favorable for action when you need a quick result. It enhances the speed of events, activities. Also good for events and actions, during which we desire changes or actions that cause changes in the future.','Ideal for all favorable purposes, except marriage; actions aimed at development, on a stable, long-term result, treatment, the start of training, starting a business, trading, borrowing money and giving, making and dressing jewelry, travelling, religious rituals, studying the scriptures.','Marriage, relationships, rude actions.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (9,9,'Ashlesha','Ashlesha','Mercury, the Divine Serpent, Shesha','Fearsome, Sharp (Tikshna)','Nakshatra is favorable for active actions, meeting with the enemy, for obtaining the results of work at any cost, for actions related to pain, damage. Sometimes painful things (temporarily) like surgery can be useful in the future.','Active actions, offensive, meeting with the enemy, victory over the enemy, competition, trials, actions related to poisons and fire, reading mystical mantras.','Any beginnings, the beginning of a trip, purchases, borrow money and give.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (10,10,'Magha','Magha','Ketu, Pitris','Fierce or Severe (Ugra)','Nakshatra is favorable for events or actions that require intensity, ferocity, recklessness. Gives the power to overcome problems with hardness, pressure. Nakshatra helps to overcome ordinary abilities and work beyond capacity.','Honoring the ancestors, contacting the authorities, starting a career, occupation of a position, holding ceremonies (weddings), solemn events, speaking in public, working with fire or weapons, poisons and chemicals, agricultural work, working with the past, studying history, ancient knowledge.','Start a trip, take money on bail, forecast, plan for the future.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (11,11,'Purva Phalguni','P.Phalguni','Venus, Bhaga','Fierce or Severe (Ugra)','Nakshatra is favorable for events or actions that require intensity, ferocity, recklessness. Gives the power to overcome problems with hardness, pressure. Nakshatra helps to overcome ordinary abilities and work beyond capacity.','Construction, cruel, harsh actions, tricks, interaction with authorities, opposition to the enemies, discussions, disputes, creativity, pleasure, entertainment, recreation, buying/selling real estate.','The beginning of large projects, to start a trip, take money on bail, start treatment or cure diseases (the diseases that have appeared in this nakshatra will be difficult to cure).','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (12,12,'Uttara Phalguni','U.Phalguni','Sun, Aryaman','Fixed or Permanent (Dhruva)','Nakshatra is favorable for the beginning of events, actions for a long period, the creation of what we do not want to further change in the future. Started at this time will bring good steady fruit.','To carry out activities designed for long-term results, marriage, acquaintance, communication, diplomacy, appeals to superiors, the opening of a store, organizations, exhibitions, long-term investments, the construction of a house, the entrance to the house, moving to a new house, buying real estate, sacred ceremonies , wearing ornaments and new clothes, planting plants.','The termination of activities, conflicts, contacts with enemies, to lend money (you can take).','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (13,13,'Hasta','Hasta','Moon, Savitr','Fast, Light(Kshipra)','Nakshatra is favorable for action when you need a quick result. It enhances the speed of events, activities. Also, good for events and actions, during which we desire changes or actions that cause changes in the future.','Beginning of training, activities related to art and craft, jewelry works, dressing new clothes and ornaments, the beginning of any activities related to the hands (carpentry, driving), homework, the beginning of building a house, treatment, surgery, manipulation with the body, cosmetic procedures, hair and nail cutting, sports, yoga, planting, buying/selling, starting a business, making profitable transactions, money transactions, travelling, transporting something, moving, changing places of residence, marriage, childcare.','Planning long-term goals and objectives, recreation, night activities.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (14,14,'Chitra','Chitra','Mars, Vishwakarma','Soft, Genentle (Mridu)','Nakshatra is favorable for the new relations development, the beginning of new affairs. Good for any gentle, joyful, emotional, entertaining and comforting affairs, events.','Treatment, improvement of health, the beginning of relations, the establishment of friendship, everything that is connected with real estate (construction, purchase, moving, repair, interior design), the beginning of education, dressing new clothes and jewelry, hair and nails cutting, ear piercing, jewelry work, art, creativity, craft, the performance of solemn rituals.','Conflicts, research, investigation and analysis.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (15,15,'Swati','Swati','Rahu, Vayu','Mobile, Temporary (Chara)','Nakshatra is favorable for events and actions, during which we desire changes or actions that will cause changes in the future, for cases involving changes or movement, for temporary, short-lived results.','One of the best nakshatras for the information spreading (advertising), business, business, trading and financial transactions, communication, contacts, acquaintances, diplomacy, training, building construction, repair, installation, installation of something, making tools or weapons, agriculture , gardening, sowing seeds, starting treatment or starvation, hair and nails cutting.','Travel, aggressive actions.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (16,16,'Visakha','Visakha','Jupiter, Indra, Agni','Mixed (Gentle, Fearsome) (Mridu-Tikshna)','Nakshatra is favorable for events with little importance. You can engage in routine activities, daily duties, but do not start new important things.','Transformations, actions where the will is required, decisiveness, where it is necessary to overcome any resistance, actions requiring mental concentration, decision-making, setting goals, activities relating to houses or lands, construction, working with metal, making vehicles, art, making ornaments, ceremonies, triumph, victory over enemies, taking medicine. You can engage in routine activities, daily duties, but do not start new important things. The one who initiates the action takes advantage of this nakshatra.','Marriage, travel, diplomatic thin talks.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (17,17,'Anuradha','Anuradha','Saturn, Mitra','Soft, Gentle (Mridu)','Nakshatra is favorable for the new relations development, the beginning of new affairs. Good for any gentle, joyful, emotional, entertaining and comforting affairs, events.','Acquaintance, establishing relationships, friendship, communication, business, contacts, conferences, meetings, hold meetings, set tasks, financial transactions, travel, relocation, immigration, any activities related to abroad, all favorable actions with movable property and real estate, first driving vehicles, studying something, occult, secret activities, the performance of solemn rituals, wearing new clothes, art, healing.','Conflicts, routine work.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (18,18,'Jyeshtha','Jyeshtha','Mercury, Indra','Fearsome, Sharp (Tikshna)','Nakshatra is favorable for active actions, meeting with the enemy, for obtaining the results of work at any cost, for actions related to pain, damage. Sometimes painful things (temporarily) like surgery can be useful in the future.','Doing active actions, offensive, meeting with the enemy, performing actions to subordinate competitors (rivals), disputes, discussions, determined conversations, to discipline oneself or someone, protection from someone or something, mystical or occult activity, family business, communication with elders.','Marriage, everything related to health, recreation, travel, diplomatic activity, shopping.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (19,19,'Mula','Mula','Ketu, Nirrti','Fearsome, Sharp (Tikshna)','Nakshatra is favorable for active actions, meeting with the enemy, for obtaining the results of work at any cost, for actions related to pain, damage. Sometimes painful things (temporarily) like surgery can be useful in the future.','Understand something, go deeper and get to the roots, to the gist, research, thinking about life, start something to study, public speaking, dig the ground, lay the foundation, the perfect day for treatment, start taking medications, herbs, surgeries, agricultural activities, planting plants, creating parks and gardens, digging wells, reservoirs or ponds, fighting enemies, agreements and breaches of agreements, marriage.','Rest, travel, diplomatic activities, lend and borrow, financial transactions, purchases.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (20,20,'Purva Ashadha','P.Ashadha','Venus, Apah','Fierce or Severe (Ugra)','Nakshatra is favorable for events or actions that require intensity, ferocity, recklessness. Gives the power to overcome problems with hardness, pressure. Nakshatra helps to overcome ordinary abilities and work beyond capacity.','Decisive actions that require will, concentration, disputes, discussions, competitions, risk, the release of someone, reconciliation and forgiveness, the inspiration of others to take decisive action, solve problems related to debts, actions associated with fire, weapons, poisons and chemical difficult work, agricultural work, agriculture, cutting and cutting trees, digging wells, reservoirs or reservoirs, travelling on water, sports.','Tactful actions and diplomacy, the completion of something, travel over land, take money on bail.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (21,21,'Uttara Ashadha','U.Ashadha','Sun, Visvedevas','Fixed or Permanent (Dhruva)','Nakshatra is favorable for the beginning of events, actions for a long period, the creation of what we do not want to further change in the future. It is necessary to carry out activities designed for long-term results. Started at this time will bring good steady fruit.','Planning, putting things in order, starting any type of activity, business, signing contracts, legal affairs, marriage, taking oaths, vows, building, laying the foundation, moving to a new house, settling or decorating a house or land, planting seeds.','Travel, completion of cases, illegal activities, rude and dishonest actions.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (22,22,'Shravana','Shravana','Moon, Vishnu','Mobile, Temporary (Chara)','Nakshatra is favorable for events and actions, during which we desire changes or actions that will cause changes in the future, for cases involving changes or movement, for temporary, short-lived results.','The most favorable of all nakshatras, new beginnings, the beginning of building a house, buying real estate and moving to a new home, the beginning of repairs, everything related to health, medical procedures, the use of medicines, prevention, starvation, training, advertising, give advice and listen advice, counselling, leading important conversations, especially on the phone, concerts and public events, acquaintance, social and organizational activities, sacred ceremonies, art, writing, composing, travelling, transportation works, transport anything, purchase of vehicles, buying new clothes, agriculture, horticulture.','Conflicts, aggressive actions, trials, the completion of something, borrow money and lend.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (23,23,'Dhanishta','Dhanishta','Mars, Vasus','Mobile, Temporary (Chara)','Nakshatra is favorable for events and actions, during which we desire changes or actions that will cause changes in the future, for cases involving changes or movement, for temporary, short-lived results.','Sacred ceremonies, large meetings, conferences, meetings, training, creative activity, art, buying clothes, expensive things, decorating, starting treatment or starvation, travelling, buying vehicles, buying real estate, starting repairs, preparing a garden plot, gardening, requiring active position, money transactions, to lend money.','Forming new partnerships, marriage, routine activities, cleaning, washing.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (24,24,'Shatabhisha','Shatabhisha','Rahu, Varuna','Mobile, Temporary (Chara)','Nakshatra is favorable for events and actions, during which we desire changes or actions that will cause changes in the future, for cases involving changes or movement, for temporary, short-lived results.','Signing of contracts, conclusion of transactions with land and real estate, the beginning of repairs, manufacturing or purchase of vehicles, travel, training, treatment, rejuvenation, cosmetology procedures, starvation, advertising, gardening.','The beginning of the activity, marriage, everything related to children, domestic activities, buying clothes and decorations, conflicts, trials, disputes, clarification of relations.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (25,25,'Purva Bhadrapada','P.Bhadra','Jupiter, Ajaikapada','Fierce or Severe (Ugra)','Nakshatra is favorable for events or actions that require intensity, ferocity, recklessness. Gives the power to overcome problems with hardness, pressure. Nakshatra helps to overcome ordinary abilities and work beyond capacity.','Risky actions, trials, conflicts, dangerous businesses, completion of cases, divorce, agricultural work, purchase of livestock, everything related to machinery, purchase, manufacture or installation of water vehicles, architectural activities, work with fire or weapons, poisons and chemical substances, pruning trees.','Start activities, travel, marriage, communication with authorities and superiors, take money on bail.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (26,26,'Uttara Bhadrapada','U.Bhadra','Saturn, Ahir Budhyana','Fixed or Permanent (Dhruva)','Nakshatra is favorable for the beginning of events, actions for a long period, the creation of what we do not want to further change in the future. It is necessary to carry out activities designed for long-term results. Started at this time will bring good steady fruit.','Marriage, the beginning of construction, the laying of the foundation, moving to a new house, making promises, vows, financial operations, treatment, anointing, baptism, art, business activity, setting up organizations, planting seeds.','Enter into contacts with enemies, trials, activities where you need to make quick decisions, give money on loans, travel, heavy physical activities, gambling.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (27,27,'Revati','Revati','Mercury, Pushan','Soft, Gentle (Mridu)','Nakshatra is favorable for the new relations development, the beginning of new affairs. Good for any gentle, joyful, emotional, entertaining and comforting affairs, events.','The beginning of any positive activity, marriage, establishment of friendship, business, career, financial transactions, buying movable and immovable property, buying and making jewelry, wearing new clothes, building, travelling, training, creativity, positively positive completion of good beginnings.','Risk-related activities, rough, aggressive actions, are very adverse for surgeries.','en');
INSERT INTO "NAKSHATRA_DESC" VALUES (28,1,'Ашвини','Ашвини','Кету, Ашвини Кумары','Быстрая, Светлая (Кшипра)','Накшатра благоприятна для действий,когда нужен быстрый результат. Усиливает скорость событий, деятельности. Также хороша для событий и действий, на протяжении которых мы желаем изменений или действий, которые вызовут изменения в будущем.','Действия, связанные со здоровьем человека (лечение, приготовление и прием лекарств,  операции (пластические), процедуры), косметологические процедуры, стрижка волос и ногтей, занятие спортом, начало обучения, изучения астрологии и духовных наук, начало бизнеса, открытие магазина, покупка, продажа (подавать объявления на продажу), покупка или продажа транспортных средств, путешествия (начало поездки), ювелирные работы, изготовление и одевание украшений, одевание новой одежды, деятельность, связанная с искусством, формирование партнерских отношений, юридические действия, начало строительство, переезд, сажать растения (особенно на растущую луну).','Завершать что-то, для любой долгосрочной деятельности, для эмоциональных событий, разговоров, брак.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (29,2,'Бхарани','Бхарани','Венера, Яма','Лютая или Суровая (Угра)','Накшатра благоприятна для событий или действий, требующих интенсивность, свирепость, безрассудство. Дает силу преодолевать проблемы с твердостью, напором. Накшатра помогает преодолевать обычные способности и работать за пределами потенциала.','Деятельность жестокая и сложная для выполнения, требующая преодоления препятствий, соревнование, победа над врагом, разрушение чего-то (развод), завершения какого-то процесса, увольнение с работы, работа с оружием сельско-хозяйственная деятельность, работа с огнем, ядами, химическими веществами, рытье колодцев, подрезание деревьев, очистительные процедуры, голодание, духовные практики, медитации, йога.','Начало деятельности, начало путешествий, покупка. Опасность отравлений, несчастных случаев, обманы. Не брать деньги под залог.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (30,3,'Криттика','Криттика','Солнце, Агни, Картикея','Смешанная (Нежная, Грозная) (Мриду-Тикшна)','Накшатра благоприятна для событий с небольшой важностью. Можно заниматься рутинной деятельностью, повседневными обязанностями, но не следует начинать новые важные дела.','Резкие, стремительные действия, конкуренция, проявление мужества, храбрости, уничтожение врага, договора, встречи (деловые), горячие обсуждения (споры), публичные выступления, судебные процессы, удаление чего-то, очистительные процедуры, выбрасывать, чистить, избавляться, выгонять, увольняться, разводиться, избавление от дурных привычек, стричься, бриться, эпиляция, работа с металлами, покупка ножей, устройств, где есть ножи, работа с оружием, деятельность, связанная с теплом (системы отопления).','Начало новых важных дел, общение, дипломатия, отдых, деятельность, связанная с водой (покупка лодки, отправляться в морское путешествие). Нельзя брать или давать деньги в долг.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (31,4,'Рохини','Рохини','Луна, Брахма','Фиксированная или Постоянная (Дхрува)','Накшатра благоприятна для начала событий, действий на длительный срок, создание того, что в дальнейшим мы не хотим, чтобы особо менялось. Следует выполнять деятельность, рассчитанную на долговременные результаты. Начатое в это время принесет хорошие устойчивые плоды.','Действия, направленные на развитие и расширение чего-либо, брак, начало отношений, действия для улучшения здоровья, долгосрочные инвестиции, накопление богатства, первое одевание украшений, работа с одеждой, орнаментом, декорациями, ювелирными изделиями, переезд в новый дом, строительство, закладка фундамента, инсталляция, установка чего-либо, инженерно-конструкторские работы, сельскохозяйственные работы, посадка семян, покупка/продажа, путешествия, принятие клятв, обетов.','Завершение чего-то, разрушение.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (32,5,'Мригашира','Мригашира','Марс, Чандра (Сома)','Мягкая, Нежная (Мриду)','Накшатра благоприятна для развития новых отношений, начала новых дел. Хороша для любых нежных, радостных, эмоциональных, развлекательных и дающих комфорт дел, событий.','Заведение дружбы, брачный союз, зачатие детей, лечение, строительство, закладка фундамента, путешествия, смена места жительства и переезд, исполнение торжественных церемоний, установление иконы или фигурки божества, религиозные дела, пожертвования, выражение почтения, начало обучения, общение, средства общения, реклама, продажа, занятия искусством, стрижка волос, ношение новых одежд.','Конфликты, принятие серьезных решений.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (33,6,'Ардра','Ардра','Раху, Рудра','Грозная, Острая (Тикшна)','Накшатра благоприятна для активных действий, встречи с противником, для получения результатов работы любой ценой, для действий, связанных с болью, нанесением ущерба. Иногда болезненные вещи (временно) типа операции, могут быть полезны в дальнейшем.','Совершение активных действий, наступление, встреча с противником, споры, разрыв договоров, разрушение, избавление от старых вещей, привычек, работа с огнем и ядами, работа с оружием.','Любые начинания, брак, путешествия, церемонии, переезд, покупки.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (34,7,'Пунарвасу','Пунарвасу','Юпитер, Адити','Подвижная, Временная (Чара)','Накшатра благоприятна для событий и действий, на протяжении которых мы желаем изменений или действий, которые вызовут изменения в будущем, для дел, связанных с переменами или движением, для временных, недолговечных по своим результатам дел. События, имевшие место в эту накшатру, повторяются.','Действия, направленные на развитие и расширение чего-либо, бизнес, отношения, начало лечения, медицинские процедуры, начало голодания, поста, диеты,  путешествие, приобретение (ремонт) транспортных средств,  строительство, закладка фундамента, начало ремонта, стрижка волос и ногтей, ювелирные работы, обучение.','Грубые действия, конфликты, споры, суды, брать и давать деньги в долг.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (35,8,'Пушья','Пушья','Сатурн, Брихаспати','Быстрая, Светлая (Кшипра)','Накшатра благоприятна для действий,когда нужен быстрый результат. Усиливает скорость событий, деятельности. Также хороша для событий и действий, на протяжении которых мы желаем изменений или действий, которые вызовут изменения в будущем.','Идеально для всех благоприятных целей, кроме брака , действия, направленные на развитие, на стабильный, долговременный результат, лечение, начало обучения, начало бизнеса, торговля, брать деньги в долг и давать, изготовление и одевание украшений, поездки, религиозные обряды, изучение священных писаний.','Брак, отношения, грубые действия.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (36,9,'Ашлеша','Ашлеша','Меркурий, Божественная змея, Шеша','Грозная, Острая (Тикшна)','Накшатра благоприятна для активных действий, встречи с противником, для получения результатов работы любой ценой, для действий, связанных с болью, нанесением ущерба. Иногда болезненные вещи (временно) типа операции, могут быть полезны в дальнейшем. ','Активные действия, наступление, встреча с противником, победа над врагом, соревнование, судебные процессы, действия, связанные с ядами и огнем, чтение мистических мантр.','Любые начинания, начало поездки, покупки, брать деньги в долг и давать.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (37,10,'Магха','Магха','Кету, Питри','Лютая или Суровая (Угра)','Накшатра благоприятна для событий или действий, требующих интенсивность, свирепость, безрассудство. Дает силу преодолевать проблемы с твердостью, напором. Накшатра помогает преодолевать обычные способности и работать за пределами потенциала.','Почитание предков, обращаться к начальству, к власти, начало карьеры, занятие должности, проведение церемоний (свадьбы), торжественных мероприятий, выступление на публике, работа с огнем или оружием, ядами и химическими веществами, сельскохозяйственные работы, работа с прошлым, изучние истории, древних знаний.','Начинать поездку, брать деньги под залог, прогнозирование, планирование будущего.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (38,11,'ПурваПхалгуни','П.Пхалгуни','Венера, Бхага','Лютая или Суровая (Угра)','Накшатра благоприятна для событий или действий, требующих интенсивность, свирепость, безрассудство. Дает силу преодолевать проблемы с твердостью, напором. Накшатра помогает преодолевать обычные способности и работать за пределами потенциала.','Строительство (конструирование), жестокие, резкие действия, уловка (хитрость), взаимодействие с органами власти, противостояние врагам, обсуждения, споры, творчество, удовольствия, развлечения, отдых, покупка/продажа недвижимого имущества.','Начало больших проектов, начинать поездку, брать деньги под залог, начало лечения или исцеления болезней (болезни, которые появились в эту накшатру, трудно будет вылечить).','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (39,12,'УттараПхалгуни','У.Пхалгуни','Солнце, Арьяман','Фиксированная или Постоянная (Дхрува)','Накшатра благоприятна для начала событий, действий на длительный срок, создание того, что в дальнейшим мы не хотим, чтобы особо менялось. Начатое в это время принесет хорошие устойчивые плоды.','Выполнять деятельность, рассчитанную на долговременные результаты, брак, знакомство, общение, дипломатия, обращение к начальству, открытие магазина, организации, выставки, долгосрочные инвестиции, начало строительства дома, церемония входа в дом, переезд в новый дом, покупка недвижимого имущества, священные церемонии, ношение украшений и новой одежды, посадка растений.','Окончание деятельности, конфликты, контакты с врагами,  давать деньги в долг (брать можно).','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (40,13,'Хаста','Хаста','Луна, Савитар','Быстрая, Светлая (Кшипра)','Накшатра благоприятна для действий,когда нужен быстрый результат. Усиливает скорость событий, деятельности. Также хороша для событий и действий, на протяжении которых мы желаем изменений или действий, которые вызовут изменения в будущем.','Начало обучения, деятельность, связанная с искусством и ремеслом, ювелирные работы, одевание новой одежды и украшений, начало любой деятельности, связанной с руками (столярные работы, вождение автомобиля), домашняя работа, начало строительства дома, лечение, операции, манипуляции с телом, косметические процедуры, стрижка волос и ногтей, спорт, йога, посадка растений, покупка/продажа, начало бизнеса, заключение выгодных сделок, денежные операции, путешествия, транспортировка чего-либо, переезд, смена места жительства, брак, уход за детьми.','Планирование долгосрочных целей и задач, отдых, ночная деятельность.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (41,14,'Читра','Читра','Марс, Вишвакарма','Мягкая, Нежная (Мриду)','Накшатра благоприятна для развития новых отношений, начала новых дел. Хороша для любых нежных, радостных, эмоциональных, развлекательных и дающих комфорт дел, событий.','Лечение, улучшение здоровья, начало отношений, заведение дружбы, все что связано с недвижимым имуществом (строительство, купля-продажа, переезд, ремонт, дизайн интерьера), начало образования, одевание новой одежды и украшений, стрижка волос и ногтей, прокалывание ушей, ювелирные работы, искусство, творчество, ремесло, исполнение торжественных ритуалов.','Конфликты, исследование, расследование и анализ.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (42,15,'Свати','Свати','Раху, Вайю','Подвижная, Временная (Чара)','Накшатра благоприятна для событий и действий, на протяжении которых мы желаем изменений или действий, которые вызовут изменения в будущем, для дел, связанных с переменами или движением, для временных, недолговечных по своим результатам дел.','Одна из лучших накшатр для распространения информации (реклама), бизнес, деловые, торговые и финансовые операции, общение, контакты, знакомства, дипломатия, обучение, строительство зданий, начало ремонта, монтаж, установка чего-либо, изготовление инструментов или оружия, сельское хозяйство, садоводство, посев семян, начало лечения или голодания, стрижка волос и ногтей.','Путешествия, агрессивные действия.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (43,16,'Вишакха','Вишакха','Юпитер, Индра, Агни','Смешанная (Нежная, Грозная) (Мриду-Тикшна)','Накшатра благоприятна для событий с небольшой важностью. Можно заниматься рутинной деятельностью, повседневными обязанностями, но не следует начинать новые важные дела.','Преобразования, действия, где требуется воля, решительность, где нужно преодолевать какое-то сопротивление, действия, требующие умственной концентрации, принятия решений, постановки целей, деятельность, касающаяся домов или земель, строительство, работы с металлом, изготовление транспортных средств, искусство, изготовление украшений, церемонии, торжества, победа над врагами, принятие лекарства. Можно заниматься рутинной деятельностью, повседневными обязанностями, но не следует начинать новые важные дела. Тот, кто инициирует действие - получает преимущества этой накшатры.','Брак, путешествия, дипломатические тонкие разговоры.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (44,17,'Анурадха','Анурадха','Сатурн, Митра','Мягкая, Нежная (Мриду)','Накшатра благоприятна для развития новых отношений, начала новых дел. Хороша для любых нежных, радостных, эмоциональных, развлекательных и дающих комфорт дел, событий.','Знакомство, завязывание отношений, дружба, общение, бизнес, контакты, конференции, встречи, проводить совещания, ставить задачи, финансовые операции, путешествия, переезд, иммиграция, любая деятельность, связанная с заграницей, все благоприятные действия c движимым имуществом и c недвижимостью, первое вождение транспортных средств, изучение чего-то, оккультная, тайная деятельность, исполнение торжественных ритуалов, ношение новых одежд, искусство, целительство.','Конфликты, рутинная работа.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (45,18,'Джйештха','Джйештха','Меркурий, Индра','Грозная, Острая (Тикшна)','Накшатра благоприятна для активных действий, встречи с противником, для получения результатов работы любой ценой, для действий, связанных с болью, нанесением ущерба. Иногда болезненные вещи (временно) типа операции, могут быть полезны в дальнейшем.','Совершение активных действий, наступление, встреча с противником, выполнение действий для подчинения конкурентов (соперников), споры, дискуссии, твердые решительные разговоры, дисциплинировать себя или кого-то, защита от кого-то или чего-то, мистическая или оккультная деятельность, семейные дела, общение со старшими.','Брак, все, что связано со здоровьем, отдых, путешествия, дипломатическая деятельность, покупки.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (46,19,'Мула','Мула','Кету, Ниррити','Грозная, Острая (Тикшна)','Накшатра благоприятна для активных действий, встречи с противником, для получения результатов работы любой ценой, для действий, связанных с болью, нанесением ущерба. Иногда болезненные вещи (временно) типа операции, могут быть полезны в дальнейшем.','Разобраться в чем то, углубиться, дойти до корней, до сути, исследовательская деятельность, размышление о жизни, начинать что-то изучать, публичные выступления, копать землю, закладывать фундамент, идеальный день для лечения, начинать принимать лекарственные препараты, травы, операции, сельскохозяйственная деятельность, посадка растений, создание парков и садов, выкапывание колодцев, резервуаров или водоемов, борьба с врагами, соглашения и нарушения соглашений, брак.','Отдых, путешествия, дипломатическая деятельность, давать и брать взаймы, финансовые операции, покупки.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (47,20,'ПурваШадха','П.Ашадха','Венера, Апас','Лютая или Суровая (Угра)','Накшатра благоприятна для событий или действий, требующих интенсивность, свирепость, безрассудство. Дает силу преодолевать проблемы с твердостью, напором. Накшатра помогает преодолевать обычные способности и работать за пределами потенциала.','Решительные действия, требующие волю, собранность и концентрацию, споры, дискуссии, соревнования, риск, освобождение кого-либо, примирение и прощение, вдохновение других к решительным действиям, решать проблемы, связанные с долгами, действия, связанные огнем, оружием, ядами и химическими веществами, сложно выполнимые, тяжелые работы, сельхозработы, земледелие, обрезание и рубка деревьев, выкапывание колодцев, резервуаров или водоемов, путешествия по воде, спорт.','Тактичные действия и дипломатия, завершение чего-то, путешествия по суше, брать деньги под залог.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (48,21,'УттараШадха','У.Ашадха','Солнце, Вишвадэвы','Фиксированная или Постоянная (Дхрува)','Накшатра благоприятна для начала событий, действий на длительный срок, создание того, что в дальнейшим мы не хотим, чтобы особо менялось. Следует выполнять деятельность, рассчитанную на долговременные результаты. Начатое в это время принесет хорошие устойчивые плоды.','Планирование, наведение порядка в делах, начало любого вида деятельности, бизнес, подписание договоров, юридические дела, брак, принятие клятв, обетов, строительство, закладка фундамента, переезд в новый дом, заселение или украшение дома или земли, посадка семян.','Путешествия, завершение дел, незаконная деятельность, грубые и нечестные действия.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (49,22,'Шравана','Шравана','Луна, Вишну','Подвижная, Временная (Чара)','Накшатра благоприятна для событий и действий, на протяжении которых мы желаем изменений или действий, которые вызовут изменения в будущем, для дел, связанных с переменами или движением, для временных, недолговечных по своим результатам дел.','Наиболее благоприятная из всех накшатр, новые начинания, начало строительства дома, покупка недвижимости и переезд в новый дом, начало ремонта, все, что связано со здоровьем, медицинские процедуры, применение лечебных средств, профилактика, голодание, обучение, реклама, давать советы и слушать советы, консультирование, вести важные разговоры, особенно по телефону, концерты и публичные мероприятия, знакомство, социальная  и организационная деятельность,   священные церемонии, искусство, писать, сочинять, путешествия, транспортировочные работы, перевозка чего-либо, приобретение транспортных средств, покупка новой одежды, сельское хозяйство, садоводство.','Конфликты, агрессивные действия, судебные процессы, завершение чего-то, брать деньги в долг и одалживать.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (50,23,'Дхаништха','Дхаништха','Марс, Васу','Подвижная, Временная (Чара)','Накшатра благоприятна для событий и действий, на протяжении которых мы желаем изменений или действий, которые вызовут изменения в будущем, для дел, связанных с переменами или движением, для временных, недолговечных по своим результатам дел.','Священные церемонии, большие собрания, конференции, митинги, обучение, творческая деятельность, искусство, покупать одежду, дорогие вещи, украшения, начало лечения или голодания, путешествия, приобретение транспортных средств, покупка недвижимости, начало ремонта, подготовка садового участка, садоводство, деятельность, требующая активной позиции, денежные операции, давать деньги в долг.','Формирование новых партнерских отношений, брак, рутинная деятельность, уборка, стирка.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (51,24,'Сатабхиша','Сатабхиша','Раху, Варуна','Подвижная, Временная (Чара)','Накшатра благоприятна для событий и действий, на протяжении которых мы желаем изменений или действий, которые вызовут изменения в будущем, для дел, связанных с переменами или движением, для временных, недолговечных по своим результатам дел.','Подписание контрактов, заключение сделок с землей и недвижимостью, начало ремонта, изготовление или приобретение транспортных средств, путешествия, обучение, лечение, омоложение, косметологические процедуры, голодание, реклама, садоводство.','Начало деятельности, брак, все, что связано с детьми, домашняя деятельность, покупка одежды и украшений, конфликты, судебные процессы, споры, выяснение отношений.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (52,25,'ПурваБхадрапада','П.Бхадра','Юпитер, Аджа Экапад','Лютая или Суровая (Угра)','Накшатра благоприятна для событий или действий, требующих интенсивность, свирепость, безрассудство. Дает силу преодолевать проблемы с твердостью, напором. Накшатра помогает преодолевать обычные способности и работать за пределами потенциала.','Рискованные действия, судебные процессы, конфликты, опасные предприятия, завершение дел, развод, сельскохозяйственные работы, закупка домашнего скота, все, что связано с техникой, закупка, изготовление или установка водных машин, архитектурная деятельность, работа с огнем или оружием, ядами и химическими веществами, подрезание деревьев.','Начало деятельности, путешествия, брак, общение с властями и начальством, брать деньги под залог.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (53,26,'УттараБхадрапада','У.Бхадра','Сатурн, Ахи Будхнья','Фиксированная или Постоянная (Дхрува)','Накшатра благоприятна для начала событий, действий на длительный срок, создание того, что в дальнейшим мы не хотим, чтобы особо менялось. Следует выполнять деятельность, рассчитанную на долговременные результаты. Начатое в это время принесет хорошие устойчивые плоды.','Брак, начало строительства, закладка фундамента, переезд в новый дом, давать обещания, обеты, клятвы, финансовые операции, лечение, помазание, крещение, искусство, деловая активность, создание организаций, посадка семян.','Вступать в контакты с врагами, судебные процессы, деятельность, где нужно принимать быстрые решения, давать деньги взаймы, путешествия, большие физические нагрузки, азартные игры.','ru');
INSERT INTO "NAKSHATRA_DESC" VALUES (54,27,'Ревати','Ревати','Меркурий, Пушан','Мягкая, Нежная (Мриду)','Накшатра благоприятна для развития новых отношений, начала новых дел. Хороша для любых нежных, радостных, эмоциональных, развлекательных и дающих комфорт дел, событий.','Начало любой позитивной деятельности, брак, заведение дружбы, бизнес, карьера, финансовые операции, покупка движимого и недвижимого имущества, покупка и изготовление драгоценностей, ношение новых одежд, строительство, путешествия, обучение, творчество, благоприятно позитивно завершить хорошие начинания.','Деятельность, связанная с риском, грубые, агрессивные действия, очень неблагоприятно для операций.','ru');
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
INSERT INTO "NITYAYOGA_DESC" VALUES (1,1,'Vishkumbha','Yama','"Jug of poison"','Unfavorable yoga, is violent. Favorable for conflicts, competitions, health-related procedures, cleansing. Don''t plan important things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (2,2,'Priti','Vishnu','"Beloved, joy"','Favorable for acquaintances, contacts, marriage, relationships, public events, conferences.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (3,3,'Ayushmaan','Chandra','"Long-lived, healthy"','Ideal for health, especially everything related to longevity: rasayana, rejuvenation of the body. Good for legal action and politics. This Yoga is favorable for any action where the stability and durability of the result obtained plays an important role.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (4,4,'Saubhagya','Brahma','"Luck, prosperity"','Favorable for almost everything. The result is easy, effortless. Speeches, presentations, conferences - if we want to achieve something. Provides additional options.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (5,5,'Shobhana','Brihaspati','"Bright, shining"','Favorable for making decisions, for acquaintances, contacts, to make a good impression. For love, romance.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (6,6,'Atiganda','Chandra','"Strongly tied knot"','Unfavorable yoga can give a huge number of problems and obstacles. Dissatisfaction with the result. Conflicts, accidents, accidents, secret, hidden. Don''t plan important things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (7,7,'Sukarmaa','Indra','"Good job"','Favorable yoga, especially for cases related to real estate.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (8,8,'Dhriti','Jala','"Stable"','A good yoga for start-up, business, money, for relations, management. Contributes craving for great luxury.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (9,9,'Shula','Sarpa','"A spear"','Unfavorable yoga, creates problems, conflicts, scandals, negative emotions. Don''t plan important things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (10,10,'Ganda','Agni','"Knot, cheek, tangle of problems, obstacles"','Unfavorable yoga. Karmic knot, a difficult entangled situation, deception, fraud is possible. Unfavorable for transactions where there is an element of trust. Don''t plan important things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (11,11,'Vriddhi','Surya','"Growth, rise, increase"','Favorable yoga for all. Overcoming obstacles, success, wealth.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (12,12,'Dhruva','Bhumi','"Stable, reliable"','Favorable yoga. Contains the blessings of Lakshmi and Saraswati. Ideal for marriage and contracts, long-term items.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (13,13,'Vyaghata','Vayu','"The obstacle leading to death"','A very unfavorable yoga, it carries conflicts, disputes, competitions. Don''t plan important things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (14,14,'Harshana','Bhaga','"Joyful, happy, prosperity"','Good for relationships, for romance. Helps to overcome difficulties and get a favorable result.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (15,15,'Vajra','Varuna','"Lightning strike"','Unfavorable yoga for important endeavors. Gives determination, defending one''s position, fighting for justice. Good yoga to defeat the influence of black magic. Achieving goals with tough methods.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (16,16,'Siddhi','Ganesh','"Completeness, prosperity, knowledge, superpowers"','Yoga is very favorable. Sidhi will help in any action when the situation is unfavorable and some kind of mystical help is needed.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (17,17,'Vyatipaata','Rudra','"Trouble, disaster"','A very unfavorable yoga. Serious danger. Harbinger of disaster. Avoid for the important things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (18,18,'Variyana','Kubera','"Comfort, best, favorable"','Good yoga for everything, especially for acquiring or buying anything that is expensive, for love.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (19,19,'Parigha','Vishwakarma','"Iron bar to close the gate"','Unfavorable yoga, gives obstacles, problems. Don''t plan important things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (20,20,'Shiva','Mitra','"Favorable"','Support from Shiva, for everything related to power, leadership. Good for training, study and for making money.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (21,21,'Siddha','Kartikeya','"Completed"','Favorable yoga in order to get maximum results in all areas of life.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (22,22,'Sadhya','Savitri','"Friendly"','Favorable yoga for diplomatic important conversations, showdown peacefully, to find a compromise.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (23,23,'Shubha','Lakshmi','"Lucky, favorable"','Favorable yoga for everything related to money and wealth, for health.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (24,24,'Shukla','Parvati','"Light, shiny"','Easy yoga for easy things. Positive, but not always stable. Changeable, impulsive.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (25,25,'Brahma','Ashwani Kumaras','"Energized, Divine Source"','Development, expansion. Good for business, for development, for creating something. Very reliable, specific, for signing contracts, etc.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (26,26,'Indra','Pitri','"Leader, king of all Devatas"','Good for everything. Especially for studying, learning something, creating something.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (27,27,'Vaidhriti','Diti','"Stopper, delay, stop"','Very unfavorable yoga, lack of support. It is favorable for conspiracies, intrigues, writing a claim. Don''t plan important things.','en');
INSERT INTO "NITYAYOGA_DESC" VALUES (28,1,'Вишкумбха','Яма','"Кувшин с ядом"','Неблагоприятная йога, носит насильственный характер. Благоприятно для конфликтов, соревнований, процедур, связанных со здоровьем, очищением. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (29,2,'Прити','Вишну','"Любимый, радость"','Благоприятно для знакомств, контактов, брака, отношений, публичных мероприятий, конференций.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (30,3,'Аюшман','Чандра','"Долго живущий, здоровый"','Идеальна для здоровья, особенно все, что связано с продолжительностью жизни: расаяны, омоложение организма. Благоприятна для юридических действий и для политики. Эта Йога благоприятна для любых действий, где важную роль играет стабильность и долговечность полученного результата.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (31,4,'Саубхагья','Брахма','"Удача, благополучие"','Благоприятна практически для всего. Результат  легко, без усилий. Выступления, презентации, конференции - если хотим чего то достичь. Дает дополнительные возможности.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (32,5,'Шобхана','Брихаспати','"Яркий, сияющий"','Благоприятна для принятия решений, для знакомства, контактов, чтобы произвести хорошее впечатление. Для любви, романтики.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (33,6,'Атиганда','Чандра','"Сильно завязанный узел"','Неблагоприятная йога может дать огромное количество проблем, препятствий. Недовольство результатом. Конфликты, несчастные случаи, аварии, тайное, скрытое. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (34,7,'Сукарма','Индра','"Хорошая работа"','Благоприятная йога, особенно для дел, связанных с недвижимостью.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (35,8,'Дхрити','Джала','"Устойчивый"','Хорошая йога для начала деятельности, для бизнеса, денег, для отношений, менеджмента, управления. Способствует тяге к большой роскоши.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (36,9,'Шула','Наги','"Копье"','Неблагоприятная йога, создает проблемы, конфликты, скандалы, негативные эмоции. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (37,10,'Ганда','Агни','"Узел, щека, клубок проблем, препятствия"','Неблагоприятная йога. Кармический узел, сложная запутанная ситуация, возможен обман, мошенничество. Неблагоприятна для сделок, где есть элемент доверия. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (38,11,'Вриддхи','Сурья','"Рост, подъем, увеличение"','Благоприятная йога для всего. Преодоление препятствий, успех, богатство.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (39,12,'Дхрува','Бхуми','"Стабильный, надежный"','Благоприятная йога. Несет в себе благословение Лакшми и Сарасвати. Идеальна для брака и контрактов, долгосрочных вещей.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (40,13,'Вьягхата','Вайю','"Препятствие, ведущая к смерти"','Очень неблагоприятная йога, несет в себе  конфликты, споры, соревнования. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (41,14,'Харшана','Бхага','"Радостный, счастливый, процветание"','Хорошо для отношений, для романтики. Помогает преодолевать сложности и получать благоприятный результат.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (42,15,'Ваджра','Варуна','"Удар молнии"','Неблагоприятная йога для важных начинаний. Дает решимость, отстаивание своей позиции, борьба за справедливость. Хорошая йога, чтобы справиться с черной магией. Достижение целей жесткими методами.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (43,16,'Сиддхи','Ганеша','"Завершенность, процветание, знания, сверхспособности"','Очень благоприятная йога. Сидхи будут помогать в любом действии, когда ситуация складывается неблагоприятная и нужна какая-то мистическая помощь.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (44,17,'Вьятипата','Рудра','"Беда, катастрофа"','Очень неблагоприятная йога. Представляет серьезную опасность. Предвестник бедствия. Избегать для важных дел.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (45,18,'Варияна','Кубера','"Комфорт, лучший, благоприятный"','Хорошая йога для всего, особенно для приобретения, покупки, или чего-то, что является дорогим, для любви.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (46,19,'Паригха','Вишвакарма','"Железный брус для закрытия ворот"','Неблагоприятная йога, дает препятствия, проблемы. Не планировать важные дела.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (47,20,'Шива','Митра','"Благоприятный"','Поддержка со стороны Шивы, для всего, что связано с властью, начальством. Хорошо для изучения, учебы и для того, чтобы зарабатывать деньги.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (48,21,'Сиддха','Картикея','"Завершенный"','Благоприятная йога для того, чтобы получить максимальный результат во всех сферах жизни.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (49,22,'Садхья','Савитри','"Дружественный"','Благоприятная йога для дипломатических важных разговоров, выяснение отношений мирным путем, для поиска компромисса.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (50,23,'Шубха','Лакшми','"Удачливый, благоприятный"','Благоприятная йога для всего, что касается денег, благосостояния, для здоровья.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (51,24,'Шукла','Парвати','"Светлый, блестящий"','Легкая йога для легких дел. Позитивная, но не всегда стабильна. Переменчивая, импульсивная.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (52,25,'Брахма','Ашвини Кумары','"Наполненный энергией, божественный источник"','Развитие, расширение. Хороша для бизнеса, для развития, создания чего-то. Очень надежная, конкретная, для подписания контрактов и т.п.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (53,26,'Индра','Питри','"Предводитель, царь всех Дэват"','Хороша для всего. Особенно для учебы, изучения чего-то, создания чего-то.','ru');
INSERT INTO "NITYAYOGA_DESC" VALUES (54,27,'Вайдхрити','Дити','"Стопор, задержка, остановка"','Очень неблагоприятная йога, отсутствие поддержки. Благоприятна для заговоров, интриг, писать претензию. Не планировать важные дела.','ru');
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
INSERT INTO "PLANET_DESC" VALUES (2,2,'Moon','en');
INSERT INTO "PLANET_DESC" VALUES (3,3,'Mars','en');
INSERT INTO "PLANET_DESC" VALUES (4,4,'Mercury','en');
INSERT INTO "PLANET_DESC" VALUES (5,5,'Jupiter','en');
INSERT INTO "PLANET_DESC" VALUES (6,6,'Venus','en');
INSERT INTO "PLANET_DESC" VALUES (7,7,'Saturn','en');
INSERT INTO "PLANET_DESC" VALUES (8,8,'Rahu','en');
INSERT INTO "PLANET_DESC" VALUES (9,9,'Ketu','en');
INSERT INTO "PLANET_DESC" VALUES (10,10,'Rahu','en');
INSERT INTO "PLANET_DESC" VALUES (11,11,'Ketu','en');
INSERT INTO "PLANET_DESC" VALUES (12,1,'Солнце','ru');
INSERT INTO "PLANET_DESC" VALUES (13,2,'Луна','ru');
INSERT INTO "PLANET_DESC" VALUES (14,3,'Марс','ru');
INSERT INTO "PLANET_DESC" VALUES (15,4,'Меркурий','ru');
INSERT INTO "PLANET_DESC" VALUES (16,5,'Юпитер','ru');
INSERT INTO "PLANET_DESC" VALUES (17,6,'Венера','ru');
INSERT INTO "PLANET_DESC" VALUES (18,7,'Сатурн','ru');
INSERT INTO "PLANET_DESC" VALUES (19,8,'Раху','ru');
INSERT INTO "PLANET_DESC" VALUES (20,9,'Кету','ru');
INSERT INTO "PLANET_DESC" VALUES (21,10,'Раху','ru');
INSERT INTO "PLANET_DESC" VALUES (22,11,'Кету','ru');
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
INSERT INTO "TRANZIT" VALUES (1,1,1,2,'');
INSERT INTO "TRANZIT" VALUES (2,1,2,2,'');
INSERT INTO "TRANZIT" VALUES (3,1,3,1,'9');
INSERT INTO "TRANZIT" VALUES (4,1,4,2,'');
INSERT INTO "TRANZIT" VALUES (5,1,5,2,'');
INSERT INTO "TRANZIT" VALUES (6,1,6,1,'12');
INSERT INTO "TRANZIT" VALUES (7,1,7,2,'');
INSERT INTO "TRANZIT" VALUES (8,1,8,2,'');
INSERT INTO "TRANZIT" VALUES (9,1,9,2,'');
INSERT INTO "TRANZIT" VALUES (10,1,10,1,'4');
INSERT INTO "TRANZIT" VALUES (11,1,11,1,'5');
INSERT INTO "TRANZIT" VALUES (12,1,12,2,'');
INSERT INTO "TRANZIT" VALUES (13,2,1,1,'5');
INSERT INTO "TRANZIT" VALUES (14,2,2,2,'');
INSERT INTO "TRANZIT" VALUES (15,2,3,1,'9');
INSERT INTO "TRANZIT" VALUES (16,2,4,2,'');
INSERT INTO "TRANZIT" VALUES (17,2,5,2,'');
INSERT INTO "TRANZIT" VALUES (18,2,6,1,'12');
INSERT INTO "TRANZIT" VALUES (19,2,7,1,'2');
INSERT INTO "TRANZIT" VALUES (20,2,8,2,'');
INSERT INTO "TRANZIT" VALUES (21,2,9,2,'');
INSERT INTO "TRANZIT" VALUES (22,2,10,1,'4');
INSERT INTO "TRANZIT" VALUES (23,2,11,1,'8');
INSERT INTO "TRANZIT" VALUES (24,2,12,2,'');
INSERT INTO "TRANZIT" VALUES (25,3,1,2,'');
INSERT INTO "TRANZIT" VALUES (26,3,2,2,'');
INSERT INTO "TRANZIT" VALUES (27,3,3,1,'12');
INSERT INTO "TRANZIT" VALUES (28,3,4,2,'');
INSERT INTO "TRANZIT" VALUES (29,3,5,2,'');
INSERT INTO "TRANZIT" VALUES (30,3,6,1,'9');
INSERT INTO "TRANZIT" VALUES (31,3,7,2,'');
INSERT INTO "TRANZIT" VALUES (32,3,8,2,'');
INSERT INTO "TRANZIT" VALUES (33,3,9,2,'');
INSERT INTO "TRANZIT" VALUES (34,3,10,2,'');
INSERT INTO "TRANZIT" VALUES (35,3,11,1,'5');
INSERT INTO "TRANZIT" VALUES (36,3,12,2,'');
INSERT INTO "TRANZIT" VALUES (37,4,1,2,'');
INSERT INTO "TRANZIT" VALUES (38,4,2,1,'5');
INSERT INTO "TRANZIT" VALUES (39,4,3,2,'');
INSERT INTO "TRANZIT" VALUES (40,4,4,1,'3');
INSERT INTO "TRANZIT" VALUES (41,4,5,2,'');
INSERT INTO "TRANZIT" VALUES (42,4,6,1,'9');
INSERT INTO "TRANZIT" VALUES (43,4,7,2,'');
INSERT INTO "TRANZIT" VALUES (44,4,8,1,'1');
INSERT INTO "TRANZIT" VALUES (45,4,9,2,'');
INSERT INTO "TRANZIT" VALUES (46,4,10,1,'8');
INSERT INTO "TRANZIT" VALUES (47,4,11,1,'12');
INSERT INTO "TRANZIT" VALUES (48,4,12,2,'');
INSERT INTO "TRANZIT" VALUES (49,5,1,2,'');
INSERT INTO "TRANZIT" VALUES (50,5,2,1,'12');
INSERT INTO "TRANZIT" VALUES (51,5,3,2,'');
INSERT INTO "TRANZIT" VALUES (52,5,4,2,'');
INSERT INTO "TRANZIT" VALUES (53,5,5,1,'4');
INSERT INTO "TRANZIT" VALUES (54,5,6,2,'');
INSERT INTO "TRANZIT" VALUES (55,5,7,1,'3');
INSERT INTO "TRANZIT" VALUES (56,5,8,2,'');
INSERT INTO "TRANZIT" VALUES (57,5,9,1,'10');
INSERT INTO "TRANZIT" VALUES (58,5,10,2,'');
INSERT INTO "TRANZIT" VALUES (59,5,11,1,'8');
INSERT INTO "TRANZIT" VALUES (60,5,12,2,'');
INSERT INTO "TRANZIT" VALUES (61,6,1,1,'8');
INSERT INTO "TRANZIT" VALUES (62,6,2,1,'7');
INSERT INTO "TRANZIT" VALUES (63,6,3,1,'1');
INSERT INTO "TRANZIT" VALUES (64,6,4,1,'10');
INSERT INTO "TRANZIT" VALUES (65,6,5,1,'9');
INSERT INTO "TRANZIT" VALUES (66,6,6,2,'');
INSERT INTO "TRANZIT" VALUES (67,6,7,2,'');
INSERT INTO "TRANZIT" VALUES (68,6,8,1,'5');
INSERT INTO "TRANZIT" VALUES (69,6,9,1,'11');
INSERT INTO "TRANZIT" VALUES (70,6,10,2,'');
INSERT INTO "TRANZIT" VALUES (71,6,11,1,'3');
INSERT INTO "TRANZIT" VALUES (72,6,12,1,'6');
INSERT INTO "TRANZIT" VALUES (73,7,1,2,'');
INSERT INTO "TRANZIT" VALUES (74,7,2,2,'');
INSERT INTO "TRANZIT" VALUES (75,7,3,1,'12');
INSERT INTO "TRANZIT" VALUES (76,7,4,2,'');
INSERT INTO "TRANZIT" VALUES (77,7,5,2,'');
INSERT INTO "TRANZIT" VALUES (78,7,6,1,'9');
INSERT INTO "TRANZIT" VALUES (79,7,7,2,'');
INSERT INTO "TRANZIT" VALUES (80,7,8,2,'');
INSERT INTO "TRANZIT" VALUES (81,7,9,2,'');
INSERT INTO "TRANZIT" VALUES (82,7,10,2,'');
INSERT INTO "TRANZIT" VALUES (83,7,11,1,'5');
INSERT INTO "TRANZIT" VALUES (84,7,12,2,'');
INSERT INTO "TRANZIT" VALUES (85,8,1,2,'');
INSERT INTO "TRANZIT" VALUES (86,8,2,2,'');
INSERT INTO "TRANZIT" VALUES (87,8,3,1,'12');
INSERT INTO "TRANZIT" VALUES (88,8,4,2,'');
INSERT INTO "TRANZIT" VALUES (89,8,5,2,'');
INSERT INTO "TRANZIT" VALUES (90,8,6,1,'9');
INSERT INTO "TRANZIT" VALUES (91,8,7,2,'');
INSERT INTO "TRANZIT" VALUES (92,8,8,2,'');
INSERT INTO "TRANZIT" VALUES (93,8,9,2,'');
INSERT INTO "TRANZIT" VALUES (94,8,10,1,'');
INSERT INTO "TRANZIT" VALUES (95,8,11,1,'5');
INSERT INTO "TRANZIT" VALUES (96,8,12,2,'');
INSERT INTO "TRANZIT" VALUES (97,9,1,2,'');
INSERT INTO "TRANZIT" VALUES (98,9,2,2,'');
INSERT INTO "TRANZIT" VALUES (99,9,3,1,'12');
INSERT INTO "TRANZIT" VALUES (100,9,4,2,'');
INSERT INTO "TRANZIT" VALUES (101,9,5,2,'');
INSERT INTO "TRANZIT" VALUES (102,9,6,1,'9');
INSERT INTO "TRANZIT" VALUES (103,9,7,2,'');
INSERT INTO "TRANZIT" VALUES (104,9,8,2,'');
INSERT INTO "TRANZIT" VALUES (105,9,9,2,'');
INSERT INTO "TRANZIT" VALUES (106,9,10,1,'');
INSERT INTO "TRANZIT" VALUES (107,9,11,1,'5');
INSERT INTO "TRANZIT" VALUES (108,9,12,2,'');
INSERT INTO "TRANZIT_DESC" VALUES (1,1,'Reducing incomes, discomfort, moving from place to place, fatigue and ill health are possible','en');
INSERT INTO "TRANZIT_DESC" VALUES (2,2,'Increase in costs, but, possibly, incomes too, eye diseases, delusions','en');
INSERT INTO "TRANZIT_DESC" VALUES (3,3,'Success, freedom from disease, overcoming obstacles, increasing energy','en');
INSERT INTO "TRANZIT_DESC" VALUES (4,4,'Problems in relationships, dishonor, general malaise, in particular, with the stomach','en');
INSERT INTO "TRANZIT_DESC" VALUES (5,5,'Sadness, confrontation, bad judgment, bodily ailment','en');
INSERT INTO "TRANZIT_DESC" VALUES (6,6,'Victory over enemies, joy, good health or recovery','en');
INSERT INTO "TRANZIT_DESC" VALUES (7,7,'Travel, wanderings, problems with place of stay, ill health','en');
INSERT INTO "TRANZIT_DESC" VALUES (8,8,'Defeat, problems in relationships, separation, humiliation','en');
INSERT INTO "TRANZIT_DESC" VALUES (9,9,'Accidents, stomach problems, feverish mental activity, recession in a career','en');
INSERT INTO "TRANZIT_DESC" VALUES (10,10,'A lot of honors, the realization of what was conceived, the recognition of society','en');
INSERT INTO "TRANZIT_DESC" VALUES (11,11,'Growth in income, respect, health and prosperity','en');
INSERT INTO "TRANZIT_DESC" VALUES (12,12,'Expenses, losses, humiliation, liberation','en');
INSERT INTO "TRANZIT_DESC" VALUES (13,13,'Good for pleasures - food, comfort and clothing, acquisitions and happiness','en');
INSERT INTO "TRANZIT_DESC" VALUES (14,14,'Less respect, money, increased barriers, problems in communication','en');
INSERT INTO "TRANZIT_DESC" VALUES (15,15,'Home happiness and affordable cash','en');
INSERT INTO "TRANZIT_DESC" VALUES (16,16,'Loss of faith in others, lack of mental balance and health, emotional breakdowns','en');
INSERT INTO "TRANZIT_DESC" VALUES (17,17,'Disorders, frustration, business failures, poor health, poor judgment ability','en');
INSERT INTO "TRANZIT_DESC" VALUES (18,18,'Happiness, income growth, victory over enemies, good health','en');
INSERT INTO "TRANZIT_DESC" VALUES (19,19,'Recognition by society, unexpected income, friendship, good for relationships','en');
INSERT INTO "TRANZIT_DESC" VALUES (20,20,'Troubles, arrest, difficulties and sorrows, health problems','en');
INSERT INTO "TRANZIT_DESC" VALUES (21,21,'Fear, difficulties and sorrows, isolation and health problems','en');
INSERT INTO "TRANZIT_DESC" VALUES (22,22,'Well-being, goals achievement, career accomplishments, favors of those in power','en');
INSERT INTO "TRANZIT_DESC" VALUES (23,23,'Prosperity, new friends and good income, happiness','en');
INSERT INTO "TRANZIT_DESC" VALUES (24,24,'Possible injuries, increased costs, obstacles due to incorrect actions','en');
INSERT INTO "TRANZIT_DESC" VALUES (25,25,'Life troubles, conflicts, injuries or illnesses','en');
INSERT INTO "TRANZIT_DESC" VALUES (26,26,'Loss of property, hostility, accidents, diseases, conflicts with others','en');
INSERT INTO "TRANZIT_DESC" VALUES (27,27,'Benefits due to decent people, cash income, achievement of the situation, victory in disputes','en');
INSERT INTO "TRANZIT_DESC" VALUES (28,28,'Increased health problems, fever, digestion problems, bleeding, low level of vitality, troubles due to friends and family','en');
INSERT INTO "TRANZIT_DESC" VALUES (29,29,'Material losses, illnesses, misunderstandings in relationships with children, quarrels, reduced life energy','en');
INSERT INTO "TRANZIT_DESC" VALUES (30,30,'Victory over enemies, income growth, resolution of family troubles, achievement of power and prestige','en');
INSERT INTO "TRANZIT_DESC" VALUES (31,31,'Conflict with a partner, fatigue, increased health problems - such as eye diseases and digestion','en');
INSERT INTO "TRANZIT_DESC" VALUES (32,32,'Increased health problems - such as bleeding and anemia, reduced well-being and trust from others, accidents, injuries, humiliation','en');
INSERT INTO "TRANZIT_DESC" VALUES (33,33,'Decline of reputation, unforeseen expenses, weakening of health, defeat','en');
INSERT INTO "TRANZIT_DESC" VALUES (34,34,'Easy money, victory in disputes, but there may be grief','en');
INSERT INTO "TRANZIT_DESC" VALUES (35,35,'Growing respect, good reputation, finding property, finding new friends','en');
INSERT INTO "TRANZIT_DESC" VALUES (36,36,'Unforeseen spending, quarrels with wife, health problems - such as eye diseases and Pitta disorders','en');
INSERT INTO "TRANZIT_DESC" VALUES (37,37,'A lot of work, bad advice, deception, false associations, conflicts, troubles while traveling','en');
INSERT INTO "TRANZIT_DESC" VALUES (38,38,'Gaining knowledge, success and wealth, but some problems with reputation','en');
INSERT INTO "TRANZIT_DESC" VALUES (39,39,'New friends, benefits, but fear of power and enemies, too many travels','en');
INSERT INTO "TRANZIT_DESC" VALUES (40,40,'Prosperity of relatives and families, income, career growth','en');
INSERT INTO "TRANZIT_DESC" VALUES (41,41,'Misunderstanding of relations with a partner and children, but personal matters are in order','en');
INSERT INTO "TRANZIT_DESC" VALUES (42,42,'Stability, recognition, quick success, popularity','en');
INSERT INTO "TRANZIT_DESC" VALUES (43,43,'Little energy, conflicts and heavy thoughts','en');
INSERT INTO "TRANZIT_DESC" VALUES (44,44,'Success, happiness, buying new things, winning','en');
INSERT INTO "TRANZIT_DESC" VALUES (45,45,'Obstacles, sickness and sad thoughts, little energy or disease','en');
INSERT INTO "TRANZIT_DESC" VALUES (46,46,'The defeat of enemies, the acquisition of money, good communication, happy unions','en');
INSERT INTO "TRANZIT_DESC" VALUES (47,47,'Gaining wealth, knowledge, happiness and friends','en');
INSERT INTO "TRANZIT_DESC" VALUES (48,48,'Spending, misunderstanding, resentment, little energy','en');
INSERT INTO "TRANZIT_DESC" VALUES (49,49,'Loss of money and ability to think, fear, indecision, misunderstanding, quarrels','en');
INSERT INTO "TRANZIT_DESC" VALUES (50,50,'Happiness, harmony of the house, cash income, rout of enemies','en');
INSERT INTO "TRANZIT_DESC" VALUES (51,51,'Moving from place to place, obstacles in their work and loss of position','en');
INSERT INTO "TRANZIT_DESC" VALUES (52,52,'Possible material losses, anxiety about relatives, but the development of good qualities','en');
INSERT INTO "TRANZIT_DESC" VALUES (53,53,'Happiness, creativity, finding a partner, children or property, developing good qualities, virtue','en');
INSERT INTO "TRANZIT_DESC" VALUES (54,54,'Defeat of the mind, sadness, friends become enemies','en');
INSERT INTO "TRANZIT_DESC" VALUES (55,55,'Happy relationships, pleasures, good income, good communication, recognition','en');
INSERT INTO "TRANZIT_DESC" VALUES (56,56,'Dissatisfaction, obstacles, complications, illness','en');
INSERT INTO "TRANZIT_DESC" VALUES (57,57,'Growth of influence, the birth of children, success in work, wealth from an unexpected source, fame, recognition','en');
INSERT INTO "TRANZIT_DESC" VALUES (58,58,'Risk of losing your place and position, loss of money and health','en');
INSERT INTO "TRANZIT_DESC" VALUES (59,59,'Stability, success, status, restoration of former positions, recovery','en');
INSERT INTO "TRANZIT_DESC" VALUES (60,60,'It is possible to pay for straightforwardness and virtue, increase in sorrows, financial losses, overstrain','en');
INSERT INTO "TRANZIT_DESC" VALUES (61,61,'Luxury, pleasure, emotional satisfaction, comfort, ornaments','en');
INSERT INTO "TRANZIT_DESC" VALUES (62,62,'Material acquisition, the birth of children, romances, family happiness','en');
INSERT INTO "TRANZIT_DESC" VALUES (63,63,'Happiness, influence, wealth and respect, fame, overcoming enemy plots','en');
INSERT INTO "TRANZIT_DESC" VALUES (64,64,'In general, prosperity, home happiness, strength and recognition','en');
INSERT INTO "TRANZIT_DESC" VALUES (65,65,'Restoring contacts with friends, growing reputation, influence, wealth and power','en');
INSERT INTO "TRANZIT_DESC" VALUES (66,66,'Fear of enemies, disease and humiliation, however, against of general prosperity on background','en');
INSERT INTO "TRANZIT_DESC" VALUES (67,67,'Difficulties in relationships, sorrows, humiliation, diseases, dangers','en');
INSERT INTO "TRANZIT_DESC" VALUES (68,68,'Happiness thanks to a partner, pleasure, but complications are also possible','en');
INSERT INTO "TRANZIT_DESC" VALUES (69,69,'Buying a new home, luxury goods, marriage, if not yet married','en');
INSERT INTO "TRANZIT_DESC" VALUES (70,70,'Rise leading to disassembly, dishonor and conflicts','en');
INSERT INTO "TRANZIT_DESC" VALUES (71,71,'Growth of incomes, benefits thanks to friends and relatives, comfort','en');
INSERT INTO "TRANZIT_DESC" VALUES (72,72,'Acquiring new friends, money, luxury goods, but also some costs','en');
INSERT INTO "TRANZIT_DESC" VALUES (73,73,'Dangers, obstacles, travel abroad, loss of money, separation from family and friends, illness, unhappiness','en');
INSERT INTO "TRANZIT_DESC" VALUES (74,74,'Sadness, loss of comfort, it is possible to gain wealth, but without the opportunity to enjoy it','en');
INSERT INTO "TRANZIT_DESC" VALUES (75,75,'Growth of wealth, increase in property and amenities, good health, overcoming obstacles and enemy intrigues','en');
INSERT INTO "TRANZIT_DESC" VALUES (76,76,'Possible separation from friends and family, confusion of mind and emotions','en');
INSERT INTO "TRANZIT_DESC" VALUES (77,77,'Separation from children, loss of money, unsuccessful speculation, misunderstanding and quarrels','en');
INSERT INTO "TRANZIT_DESC" VALUES (78,78,'Overcoming enemies and diseases, profitable relationships, acquisition of property','en');
INSERT INTO "TRANZIT_DESC" VALUES (79,79,'Separation from a partner and children, defective thinking, aimless throwing','en');
INSERT INTO "TRANZIT_DESC" VALUES (80,80,'Confrontation, health problems, possible humiliation, wrong actions, injuries, losses','en');
INSERT INTO "TRANZIT_DESC" VALUES (81,81,'Lack of luck, financial loss, hostility, unexpected problems, difficulties in spiritual life','en');
INSERT INTO "TRANZIT_DESC" VALUES (82,82,'Possibility to find a job, but the loss of reputation, wealth and status','en');
INSERT INTO "TRANZIT_DESC" VALUES (83,83,'The acquisition of wealth and property, status and position, but possible wrong actions','en');
INSERT INTO "TRANZIT_DESC" VALUES (84,84,'Costs, sorrows, soreness, humiliation and grief','en');
INSERT INTO "TRANZIT_DESC" VALUES (85,85,'Disease and fear','en');
INSERT INTO "TRANZIT_DESC" VALUES (86,86,'Loss of wealth, conflicts and misunderstandings','en');
INSERT INTO "TRANZIT_DESC" VALUES (87,87,'Happiness and good news','en');
INSERT INTO "TRANZIT_DESC" VALUES (88,88,'Disease, danger and despondency','en');
INSERT INTO "TRANZIT_DESC" VALUES (89,89,'Financial loss and suffering','en');
INSERT INTO "TRANZIT_DESC" VALUES (90,90,'Pleasure and happy relationships','en');
INSERT INTO "TRANZIT_DESC" VALUES (91,91,'Loss and fear','en');
INSERT INTO "TRANZIT_DESC" VALUES (92,92,'Danger to health or life','en');
INSERT INTO "TRANZIT_DESC" VALUES (93,93,'Conflict, sullen thoughts and losses','en');
INSERT INTO "TRANZIT_DESC" VALUES (94,94,'Hostility, obstacles','en');
INSERT INTO "TRANZIT_DESC" VALUES (95,95,'Happiness and big money','en');
INSERT INTO "TRANZIT_DESC" VALUES (96,96,'Wastes and Dangers','en');
INSERT INTO "TRANZIT_DESC" VALUES (97,97,'Disease and fear','en');
INSERT INTO "TRANZIT_DESC" VALUES (98,98,'Loss of wealth, conflicts and misunderstandings','en');
INSERT INTO "TRANZIT_DESC" VALUES (99,99,'Happiness and good news','en');
INSERT INTO "TRANZIT_DESC" VALUES (100,100,'Disease, danger and despondency','en');
INSERT INTO "TRANZIT_DESC" VALUES (101,101,'Financial loss and suffering','en');
INSERT INTO "TRANZIT_DESC" VALUES (102,102,'Pleasure and happy relationships','en');
INSERT INTO "TRANZIT_DESC" VALUES (103,103,'Loss and fear','en');
INSERT INTO "TRANZIT_DESC" VALUES (104,104,'Danger to health or life','en');
INSERT INTO "TRANZIT_DESC" VALUES (105,105,'Conflict, sullen thoughts and losses','en');
INSERT INTO "TRANZIT_DESC" VALUES (106,106,'Hostility, obstacles','en');
INSERT INTO "TRANZIT_DESC" VALUES (107,107,'Happiness and big money','en');
INSERT INTO "TRANZIT_DESC" VALUES (108,108,'Wastes and Dangers','en');
INSERT INTO "TRANZIT_DESC" VALUES (109,1,'Сокращение доходов, дискомфорт, возможны перемещения с места на место, усталость и плохое здоровье','ru');
INSERT INTO "TRANZIT_DESC" VALUES (110,2,'Возрастание расходов, но, возможно, и доходов тоже, болезни глаз, заблуждения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (111,3,'Успех, свобода от болезней, преодоление преград, возрастание энергии','ru');
INSERT INTO "TRANZIT_DESC" VALUES (112,4,'Проблемы во взаимоотношениях, бесчестие, общее недомогание, в частности, с желудком','ru');
INSERT INTO "TRANZIT_DESC" VALUES (113,5,'Печаль, противостояние, плохое суждение, телесное недомогание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (114,6,'Победа над врагами, радость, хорошее здоровье или выздоровление','ru');
INSERT INTO "TRANZIT_DESC" VALUES (115,7,'Путешествия, странствия, проблемы в месте своего пребывания, плохое здоровье','ru');
INSERT INTO "TRANZIT_DESC" VALUES (116,8,'Поражение, проблемы во взаимоотношениях, разлука, унижение','ru');
INSERT INTO "TRANZIT_DESC" VALUES (117,9,'Несчастные случаи, проблемы с желудком, лихорадочная умственная деятельность, спад в карьере','ru');
INSERT INTO "TRANZIT_DESC" VALUES (118,10,'Много почестей, реализация задуманного, признание обществом','ru');
INSERT INTO "TRANZIT_DESC" VALUES (119,11,'Рост доходов, уважение, здоровье и процветание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (120,12,'Расходы, потери, унижение, освобождение','ru');
INSERT INTO "TRANZIT_DESC" VALUES (121,13,'Хорошо для удовольствий – пища, удобства и одежда, приобретения и счастье','ru');
INSERT INTO "TRANZIT_DESC" VALUES (122,14,'Меньше уважения, денег, увеличение преград, проблемы в общении','ru');
INSERT INTO "TRANZIT_DESC" VALUES (123,15,'Домашнее счастье и доступные денежные средства','ru');
INSERT INTO "TRANZIT_DESC" VALUES (124,16,'Утрата веры в других, недостаток умственного равновесия, здоровья, эмоциональные срывы','ru');
INSERT INTO "TRANZIT_DESC" VALUES (125,17,'Расстройства, разочарование, неудачи в бизнесе, слабое здоровье, плохая способность суждения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (126,18,'Счастье, рост доходов, победа над врагами, хорошее здоровье','ru');
INSERT INTO "TRANZIT_DESC" VALUES (127,19,'Признание обществом, неожиданные доходы, дружба, хорошо для отношений','ru');
INSERT INTO "TRANZIT_DESC" VALUES (128,20,'Беды, арест, трудности и печали, проблемы со здоровьем','ru');
INSERT INTO "TRANZIT_DESC" VALUES (129,21,'Страх, трудности и печали, изоляция и проблемы со здоровьем','ru');
INSERT INTO "TRANZIT_DESC" VALUES (130,22,'Благополучие, достижение целей, карьерные свершения, благосклонность власть имущих','ru');
INSERT INTO "TRANZIT_DESC" VALUES (131,23,'Процветание, новые друзья и хороший доход, счастье','ru');
INSERT INTO "TRANZIT_DESC" VALUES (132,24,'Возможны ранения, увеличение расходов, препятствия из-за неверных действий','ru');
INSERT INTO "TRANZIT_DESC" VALUES (133,25,'Жизненные неурядицы, конфликты, ранения или болезни','ru');
INSERT INTO "TRANZIT_DESC" VALUES (134,26,'Потеря собственности, вражда, несчастные случаи, болезни, конфликты с окружающими','ru');
INSERT INTO "TRANZIT_DESC" VALUES (135,27,'Выгоды благодаря достойным людям, денежные доходы, достижение положения, победа в спорах','ru');
INSERT INTO "TRANZIT_DESC" VALUES (136,28,'Увеличение проблем со здоровьем, возможны лихорадка, проблемы с пищеварением, кровотечения, низкий уровень жизненных сил, беды из-за друзей и домочадцев','ru');
INSERT INTO "TRANZIT_DESC" VALUES (137,29,'Материальные потери, болезни, непонимание в отношениях с детьми, ссоры, пониженная жизненная энергия','ru');
INSERT INTO "TRANZIT_DESC" VALUES (138,30,'Победа над врагами, рост дохода, разрешение семейных неурядиц, достижение власти и престижа','ru');
INSERT INTO "TRANZIT_DESC" VALUES (139,31,'Конфликт с партнёром, усталость, увеличение проблем со здоровьем – такие, как болезни глаз и пищеварения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (140,32,'Увеличение проблем со здоровьем – такие, как кровотечения и анемия, снижение благополучия и доверия со стороны других, аварии, ранения, унижение','ru');
INSERT INTO "TRANZIT_DESC" VALUES (141,33,'Упадок репутации, непредвиденные расходы, ослабление здоровья, поражение','ru');
INSERT INTO "TRANZIT_DESC" VALUES (142,34,'Шальные деньги, победа в спорах, но возможны огорчения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (143,35,'Рост уважения, хорошая репутация, обретение собственности, обретение новых друзей','ru');
INSERT INTO "TRANZIT_DESC" VALUES (144,36,'Непредвиденные траты, ссоры с женой, проблемы со здоровьем – такие, как болезни глаз и расстройства Питты','ru');
INSERT INTO "TRANZIT_DESC" VALUES (145,37,'Много работы, плохой совет, обман, ложные ассоциации, конфликты, беды во время путешествия','ru');
INSERT INTO "TRANZIT_DESC" VALUES (146,38,'Обретение знаний, успех и богатство, но некоторые проблемы с репутацией','ru');
INSERT INTO "TRANZIT_DESC" VALUES (147,39,'Новые друзья, выгоды, но страх перед властью и врагами, излишне много путешествий','ru');
INSERT INTO "TRANZIT_DESC" VALUES (148,40,'Процветание родственников и семьи, доходы, карьерный рост','ru');
INSERT INTO "TRANZIT_DESC" VALUES (149,41,'Непонимание в отношениях с партнёром и детьми, но личные дела в полном порядке','ru');
INSERT INTO "TRANZIT_DESC" VALUES (150,42,'Стабильность, признание, быстрый успех, популярность','ru');
INSERT INTO "TRANZIT_DESC" VALUES (151,43,'Мало энергии, конфликты и тяжеловесные мысли','ru');
INSERT INTO "TRANZIT_DESC" VALUES (152,44,'Успех, счастье, покупка новых вещей, победа','ru');
INSERT INTO "TRANZIT_DESC" VALUES (153,45,'Препятствия, болезнь и печальные мысли, мало энергии или болезнь','ru');
INSERT INTO "TRANZIT_DESC" VALUES (154,46,'Разгром врагов, обретение денег, хорошее общение, счастливые союзы','ru');
INSERT INTO "TRANZIT_DESC" VALUES (155,47,'Обретение богатства, знаний, счастья и друзей','ru');
INSERT INTO "TRANZIT_DESC" VALUES (156,48,'Траты, непонимание, обиды, мало энергии','ru');
INSERT INTO "TRANZIT_DESC" VALUES (157,49,'Потеря денег и способности думать, страх, нерешительность, непонимание, ссоры','ru');
INSERT INTO "TRANZIT_DESC" VALUES (158,50,'Счастье, гармония дома, денежные поступления, разгром врагов','ru');
INSERT INTO "TRANZIT_DESC" VALUES (159,51,'Перемещение с места на место, препятствия в своей работе и утрата положения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (160,52,'Возможны материальные потери, беспокойство о родственниках, но развитие хороших качеств','ru');
INSERT INTO "TRANZIT_DESC" VALUES (161,53,'Счастье, творчество, обретение партнёра, детей или собственности, развитие хороших качеств, добродетель','ru');
INSERT INTO "TRANZIT_DESC" VALUES (162,54,'Поражение ума, печаль, друзья становятся врагами','ru');
INSERT INTO "TRANZIT_DESC" VALUES (163,55,'Счастливые взаимоотношения, удовольствия, хороший доход, хорошее общение, признание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (164,56,'Неудовлетворённость, препятствия, осложнения, болезнь','ru');
INSERT INTO "TRANZIT_DESC" VALUES (165,57,'Рост влияния, рождение детей, успех в работе, богатство от неожиданного источника, слава, признание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (166,58,'Риск потерять место и положение, утрата денег и здоровья','ru');
INSERT INTO "TRANZIT_DESC" VALUES (167,59,'Стабильность, успех, статус, восстановление прежних позиций, выздоровление','ru');
INSERT INTO "TRANZIT_DESC" VALUES (168,60,'Возможна расплата за прямоту и добродетель, увеличение горестей, финансовые потери, перенапряжение','ru');
INSERT INTO "TRANZIT_DESC" VALUES (169,61,'Наслаждения, роскошь, удовольствия, эмоциональное удовлетворение, комфорт, украшения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (170,62,'Материальные приобретения, рождение детей, романы, семейное счастье','ru');
INSERT INTO "TRANZIT_DESC" VALUES (171,63,'Счастье, влияние, богатство и уважение, известность, преодоление вражеских козней','ru');
INSERT INTO "TRANZIT_DESC" VALUES (172,64,'В целом процветание, домашнее счастье, сила и признание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (173,65,'Возобновление контактов с друзьями, рост репутации, влияния, богатства и власти','ru');
INSERT INTO "TRANZIT_DESC" VALUES (174,66,'Страх перед врагами, болезнь и унижение, впрочем, на фоне общего процветания','ru');
INSERT INTO "TRANZIT_DESC" VALUES (175,67,'Трудности во взаимоотношениях, горести, унижение, болезни, опасности','ru');
INSERT INTO "TRANZIT_DESC" VALUES (176,68,'Счастье благодаря партнёру, удовольствие, но возможны и осложнения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (177,69,'Покупка нового дома, предметов роскоши, женитьба, если ещё не женат','ru');
INSERT INTO "TRANZIT_DESC" VALUES (178,70,'Возвышение, приводящее к разборкам, бесчестие и конфликты','ru');
INSERT INTO "TRANZIT_DESC" VALUES (179,71,'Рост доходов, выгоды благодаря друзьям и родственникам, комфорт','ru');
INSERT INTO "TRANZIT_DESC" VALUES (180,72,'Обретение новых друзей, денег, предметов роскоши, но и некоторые затраты','ru');
INSERT INTO "TRANZIT_DESC" VALUES (181,73,'Опасность, препятствия, путешествия за границу, потеря денег, разлука с семьёй и друзьями, болезнь, несчастье','ru');
INSERT INTO "TRANZIT_DESC" VALUES (182,74,'Печаль, утрата комфорта, возможно обретение богатства, но без возможности насладиться им','ru');
INSERT INTO "TRANZIT_DESC" VALUES (183,75,'Рост богатства, увеличение собственности и жизненных удобств, хорошее здоровье, преодоление препятствий и вражеских козней','ru');
INSERT INTO "TRANZIT_DESC" VALUES (184,76,'Возможна разлука с друзьями и домашними, смятение ума и эмоций','ru');
INSERT INTO "TRANZIT_DESC" VALUES (185,77,'Разлука с детьми, потеря денег, неудачные спекуляции, непонимание и ссоры','ru');
INSERT INTO "TRANZIT_DESC" VALUES (186,78,'Преодоление врагов и болезней, выгодные взаимоотношения, обретение собственности','ru');
INSERT INTO "TRANZIT_DESC" VALUES (187,79,'Разлука с партнёром и детьми, ущербное мышление, бесцельные метания','ru');
INSERT INTO "TRANZIT_DESC" VALUES (188,80,'Противостояние, проблемы со здоровьем, возможно унижение, неверные действия, ранения, потери','ru');
INSERT INTO "TRANZIT_DESC" VALUES (189,81,'Отсутствие удачи, финансовые потери, враждебность, неожиданные проблемы, трудности в духовной жизни','ru');
INSERT INTO "TRANZIT_DESC" VALUES (190,82,'Возможно нахождение работы, но потеря репутации, богатства и статуса','ru');
INSERT INTO "TRANZIT_DESC" VALUES (191,83,'Обретение богатства и собственности, статуса и положения, но возможны неверные действия','ru');
INSERT INTO "TRANZIT_DESC" VALUES (192,84,'Расходы, горести, болезненность, унижение и огорчения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (193,85,'Болезнь и страх','ru');
INSERT INTO "TRANZIT_DESC" VALUES (194,86,'Потеря богатства, конфликты и непонимание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (195,87,'Счастье и хорошие новости','ru');
INSERT INTO "TRANZIT_DESC" VALUES (196,88,'Болезнь, опасность и уныние','ru');
INSERT INTO "TRANZIT_DESC" VALUES (197,89,'Финансовые потери и страдание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (198,90,'Удовольствие и счастливые взаимоотношения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (199,91,'Потери и страх','ru');
INSERT INTO "TRANZIT_DESC" VALUES (200,92,'Опасность здоровью или жизни','ru');
INSERT INTO "TRANZIT_DESC" VALUES (201,93,'Конфликт, угрюмые мысли и потери','ru');
INSERT INTO "TRANZIT_DESC" VALUES (202,94,'Враждебность, преграды','ru');
INSERT INTO "TRANZIT_DESC" VALUES (203,95,'Счастье и большие деньги','ru');
INSERT INTO "TRANZIT_DESC" VALUES (204,96,'Траты и опасности','ru');
INSERT INTO "TRANZIT_DESC" VALUES (205,97,'Болезнь и страх','ru');
INSERT INTO "TRANZIT_DESC" VALUES (206,98,'Потеря богатства, конфликты и непонимание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (207,99,'Счастье и хорошие новости','ru');
INSERT INTO "TRANZIT_DESC" VALUES (208,100,'Болезнь, опасность и уныние','ru');
INSERT INTO "TRANZIT_DESC" VALUES (209,101,'Финансовые потери и страдание','ru');
INSERT INTO "TRANZIT_DESC" VALUES (210,102,'Удовольствие и счастливые взаимоотношения','ru');
INSERT INTO "TRANZIT_DESC" VALUES (211,103,'Потери и страх','ru');
INSERT INTO "TRANZIT_DESC" VALUES (212,104,'Опасность здоровью или жизни','ru');
INSERT INTO "TRANZIT_DESC" VALUES (213,105,'Конфликт, угрюмые мысли и потери','ru');
INSERT INTO "TRANZIT_DESC" VALUES (214,106,'Враждебность, преграды','ru');
INSERT INTO "TRANZIT_DESC" VALUES (215,107,'Счастье и большие деньги','ru');
INSERT INTO "TRANZIT_DESC" VALUES (216,108,'Траты и опасности','ru');
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
INSERT INTO "ZODIAK" VALUES (1,'ARI');
INSERT INTO "ZODIAK" VALUES (2,'TAU');
INSERT INTO "ZODIAK" VALUES (3,'GEM');
INSERT INTO "ZODIAK" VALUES (4,'CNC');
INSERT INTO "ZODIAK" VALUES (5,'LEO');
INSERT INTO "ZODIAK" VALUES (6,'VIR');
INSERT INTO "ZODIAK" VALUES (7,'LIB');
INSERT INTO "ZODIAK" VALUES (8,'SCO');
INSERT INTO "ZODIAK" VALUES (9,'SGR');
INSERT INTO "ZODIAK" VALUES (10,'CAP');
INSERT INTO "ZODIAK" VALUES (11,'AQR');
INSERT INTO "ZODIAK" VALUES (12,'PSC');
INSERT INTO "ZODIAK_DESC" VALUES (1,1,'Aries','en');
INSERT INTO "ZODIAK_DESC" VALUES (2,2,'Taurus','en');
INSERT INTO "ZODIAK_DESC" VALUES (3,3,'Gemini','en');
INSERT INTO "ZODIAK_DESC" VALUES (4,4,'Cancer','en');
INSERT INTO "ZODIAK_DESC" VALUES (5,5,'Leo','en');
INSERT INTO "ZODIAK_DESC" VALUES (6,6,'Virgo','en');
INSERT INTO "ZODIAK_DESC" VALUES (7,7,'Libra','en');
INSERT INTO "ZODIAK_DESC" VALUES (8,8,'Scorpio','en');
INSERT INTO "ZODIAK_DESC" VALUES (9,9,'Sagittarius','en');
INSERT INTO "ZODIAK_DESC" VALUES (10,10,'Capricorn','en');
INSERT INTO "ZODIAK_DESC" VALUES (11,11,'Aquarius','en');
INSERT INTO "ZODIAK_DESC" VALUES (12,12,'Pisces','en');
INSERT INTO "ZODIAK_DESC" VALUES (13,1,'Овен','ru');
INSERT INTO "ZODIAK_DESC" VALUES (14,2,'Телец','ru');
INSERT INTO "ZODIAK_DESC" VALUES (15,3,'Близнецы','ru');
INSERT INTO "ZODIAK_DESC" VALUES (16,4,'Рак','ru');
INSERT INTO "ZODIAK_DESC" VALUES (17,5,'Лев','ru');
INSERT INTO "ZODIAK_DESC" VALUES (18,6,'Дева','ru');
INSERT INTO "ZODIAK_DESC" VALUES (19,7,'Весы','ru');
INSERT INTO "ZODIAK_DESC" VALUES (20,8,'Скорпион','ru');
INSERT INTO "ZODIAK_DESC" VALUES (21,9,'Стрелец','ru');
INSERT INTO "ZODIAK_DESC" VALUES (22,10,'Козерог','ru');
INSERT INTO "ZODIAK_DESC" VALUES (23,11,'Водолей','ru');
INSERT INTO "ZODIAK_DESC" VALUES (24,12,'Рыбы','ru');
COMMIT;
