using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TableComparator.TableStructure;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace TableComparator
{
    class Program
    {
        static MySqlCommand command;
        static MySqlConnection connection;

        static List<Creature> creature = new List<Creature>(); //normal data
        static List<Creature> creatureS = new List<Creature>(); //data from sniff

        static List<ModelInfo> model = new List<ModelInfo>();
        static List<ModelInfo> modelS = new List<ModelInfo>();

        static List<GameobjectTemplate> go = new List<GameobjectTemplate>();
        static List<GameobjectTemplate> gos = new List<GameobjectTemplate>();

        static uint BadQuery = 0;
        static string table = "<null>";
        static string field = "<null>";

        static void Main(string[] args)
        {
            using (StreamReader reader = new StreamReader("config.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                Config config = (Config)serializer.Deserialize(reader);

                string connectionInfo = string.Format("host={0};port='{1}';database='{2}';UserName='{3}';Password='{4}';Connection Timeout='{5}'",
                    config.host, config.port, config.dataBase, config.userName, config.password, config.connectionTimeOut);

                connection = new MySqlConnection(connectionInfo);
            }

            Console.Title = "Table Comparator v1.0";
            AppDomain.CurrentDomain.UnhandledException +=
                (o, e) => CrashReport(e.ExceptionObject.ToString());

            if (!ConnectionIsOpen)
                return;

            Console.WriteLine("==========||====================================================||=========||");
            Console.WriteLine("Statistics|| Templates: || Bad fields:  || Table:               || %       ||");

            SelectCreature();
            SelectModel();
            SelectGameObject();

            Console.WriteLine("==========||====================================================||=========||");
        }

        static bool SelectCreature()
        {
            table = "creature_template";
            field = "entry";
            string query = "SELECT creature_template.entry, creature_template.speed_run, creature_template.speed_walk, creature_template.faction_A, creature_template.dynamicflags, creature_template.unit_flags, creature_template.rangeattacktime, creature_template.baseattacktime, creature_template.scale, creature_template.unit_class, creature_template.mindmg, creature_template.maxdmg, creature_template.attackpower, creature_template.rangedattackpower, creature_template.dmg_multiplier, creature_template.VehicleId, creature_template_sniff.entry, creature_template_sniff.speed_run, creature_template_sniff.speed_walk, creature_template_sniff.faction_A, creature_template_sniff.dynamicflags, creature_template_sniff.unit_flags, creature_template_sniff.rangeattacktime, creature_template_sniff.baseattacktime, creature_template_sniff.scale, creature_template_sniff.unit_class, creature_template_sniff.mindmg, creature_template_sniff.maxdmg, creature_template_sniff.attackpower, creature_template_sniff.rangedattackpower, creature_template_sniff.dmg_multiplier, creature_template_sniff.VehicleId FROM creature_template INNER JOIN creature_template_sniff ON creature_template.`entry` = creature_template_sniff.`entry` ORDER BY creature_template.entry";
            command = new MySqlCommand(query, connection);
            using (MySqlDataReader db = command.ExecuteReader())
            {
                while (db.Read())
                {
                    Creature cr = new Creature();
                    Creature crs = new Creature();

                    cr.entry = db[0].ToUint32();
                    cr.speed_run = db[1].ToFloat();
                    cr.speed_walk = db[2].ToFloat();
                    cr.faction = db[3].ToUint32();
                    cr.dynamicFlags = db[4].ToUint32();
                    cr.unit_flag = db[5].ToUint32();
                    cr.rangeAttackTime = db[6].ToUint32();
                    cr.baseAttackTime = db[7].ToUint32();
                    cr.scale = db[8].ToFloat();
                    cr.unit_class = db[9].ToUint32();
                    cr.minDamage = db[10].ToFloat();
                    cr.maxDamage = db[11].ToFloat();
                    cr.attackPower = db[12].ToUint32();
                    cr.rangeAttackPower = db[13].ToUint32();
                    cr.dmgMultiplier = db[14].ToUint32();
                    cr.vehicleId = db[15].ToUInt16();

                    crs.entry = db[16].ToUint32();
                    crs.speed_run = db[17].ToFloat();
                    crs.speed_walk = db[18].ToFloat();
                    crs.faction = db[19].ToUint32();
                    crs.dynamicFlags = db[20].ToUint32();
                    crs.unit_flag = db[21].ToUint32();
                    crs.rangeAttackTime = db[22].ToUint32();
                    crs.baseAttackTime = db[23].ToUint32();
                    crs.scale = db[24].ToFloat();
                    crs.unit_class = db[25].ToUint32();
                    crs.minDamage = db[26].ToFloat();
                    crs.maxDamage = db[27].ToFloat();
                    crs.attackPower = db[28].ToUint32();
                    crs.rangeAttackPower = db[29].ToUint32();
                    crs.dmgMultiplier = db[30].ToUint32();
                    crs.vehicleId = db[31].ToUInt16();

                    creature.Add(cr);
                    creatureS.Add(crs);
                }
            }
            return CreatureCompare(); ;
        }

        static bool CreatureCompare()
        {
            using (StreamWriter writer = new StreamWriter("creature_template.sql", true))
            {
                uint entry = 0;
                int counts = creatureS.Count;

                for (int i = 0; i < counts; ++i)
                {
                    Creature cr = creature[i];
                    Creature crs = creatureS[i];
                    entry = crs.entry;

                    if (!object.Equals(cr.speed_run, crs.speed_run))
                        Query("speed_run", crs.speed_run, entry, writer);

                    if (!object.Equals(cr.speed_walk, crs.speed_walk))
                        Query("speed_walk", crs.speed_walk, entry, writer);

                    if (!object.Equals(cr.vehicleId, crs.vehicleId))
                        Query("vehicle_id", crs.vehicleId, entry, writer);

                    if (!object.Equals(cr.unit_class, crs.unit_class))
                        Query("unit_class", crs.unit_class, entry, writer);

                    if (!object.Equals(cr.unit_flag, crs.unit_flag))
                        Query("unit_flags", crs.unit_flag, entry, writer);

                    if (!object.Equals(cr.faction, crs.faction))
                        Query("faction_A", "faction_H", crs.faction, entry, writer);

                    if (!object.Equals(cr.dynamicFlags, crs.dynamicFlags))
                        Query("dynamicflags", crs.dynamicFlags, entry, writer);

                    if (!object.Equals(cr.scale, crs.scale))
                        Query("scale", crs.scale, entry, writer);

                    if (!object.Equals(cr.maxDamage, crs.maxDamage))
                        Query("maxdmg", crs.maxDamage, entry, writer);

                    if (!object.Equals(cr.minDamage, crs.minDamage))
                        Query("mindmg", crs.minDamage, entry, writer);

                    if (!object.Equals(cr.dmgMultiplier, crs.dmgMultiplier))
                        Query("dmg_multiplier", crs.dmgMultiplier, entry, writer);

                    if (!object.Equals(cr.attackPower, crs.attackPower))
                        Query("attackpower", crs.attackPower, entry, writer);

                    if (!object.Equals(cr.rangeAttackPower, crs.rangeAttackPower))
                        Query("rangedattackpower", crs.rangeAttackPower, entry, writer);

                    if (!object.Equals(cr.rangeAttackTime, crs.rangeAttackTime))
                        Query("rangeattacktime", crs.rangeAttackTime, entry, writer);

                    if (!object.Equals(cr.baseAttackTime, crs.baseAttackTime))
                        Query("baseattacktime", crs.baseAttackTime, entry, writer);

                    if (i == (counts - 1))
                        Console.WriteLine("==========|| {0,-11}|| {1,-13}|| {2,-21}|| {3}||", counts, BadQuery, table, ((float)BadQuery / (float)counts) * 100);
                }
            }

            return false;
        }

        static bool SelectModel()
        {
            table = "creature_model_info";
            field = "modelId";

            string query = "SELECT creature_model_info.`modelid`, creature_model_info.`bounding_radius`, creature_model_info.`combat_reach`, creature_model_info.`gender`, creature_model_info_sniff.`modelid`, creature_model_info_sniff.`bounding_radius`, creature_model_info_sniff.`combat_reach`, creature_model_info_sniff.`gender` FROM creature_model_info INNER JOIN creature_model_info_sniff ON creature_model_info.`modelid` = creature_model_info_sniff.`modelid` ORDER BY creature_model_info.`modelid`";
            command = new MySqlCommand(query, connection);

            using (MySqlDataReader db = command.ExecuteReader())
            {
                while (db.Read())
                {
                    ModelInfo mod = new ModelInfo();
                    ModelInfo modS = new ModelInfo();

                    mod.modelid = db[0].ToUint32();
                    mod.bounding_radius = db[1].ToFloat();
                    mod.combat_reach = db[2].ToFloat();
                    mod.gender = db[3].ToSByte();

                    modS.modelid = db[4].ToUint32();
                    modS.bounding_radius = db[5].ToFloat();
                    modS.combat_reach = db[6].ToFloat();
                    modS.gender = db[7].ToSByte();

                    model.Add(mod);
                    modelS.Add(modS);
                }
            }
            return CompareModel();
        }

        static bool CompareModel()
        {
            using (StreamWriter writer = new StreamWriter("creature_model_info.sql", true))
            {
                uint modelId = 0;
                int counts = modelS.Count;

                for (int i = 0; i < counts; ++i)
                {
                    ModelInfo mod = model[i];
                    ModelInfo mods = modelS[i];
                    modelId = mod.modelid;

                    if (!object.Equals(mod.bounding_radius, mods.bounding_radius))
                        Query("bounding_radius", mods.bounding_radius, modelId, writer);

                    if (!object.Equals(mod.combat_reach, mods.combat_reach))
                        Query("combat_reach", mods.combat_reach, modelId, writer);

                    if (!object.Equals(mod.gender, mods.gender))
                        Query("gender", mods.gender, modelId, writer);

                    if (i == (counts - 1))
                        Console.WriteLine("==========|| {0,-11}|| {1,-13}|| {2,-21}|| {3}||", counts, BadQuery, table, ((float)BadQuery / (float)counts) * 100);
                }
            }

            return false;
        }

        static bool SelectGameObject()
        {
            table = "gameobject_template";
            field = "entry";

            string query = "SELECT gameobject_templat.`entry`, gameobject_templat.`faction`, gameobject_templat.`flags`, gameobject_templat_sniff.`entry`, gameobject_templat_sniff.`faction`, gameobject_templat_sniff.`flags` FROM gameobject_templat INNER JOIN gameobject_templat_sniff ON gameobject_templat.`entry` = gameobject_templat_sniff.`entry` ORDER BY gameobject_templat.`entry`";
            command = new MySqlCommand(query, connection);

            using (MySqlDataReader db = command.ExecuteReader())
            {
                while (db.Read())
                {
                    GameobjectTemplate g = new GameobjectTemplate();
                    GameobjectTemplate gs = new GameobjectTemplate();

                    g.entry = db[0].ToUint32();
                    g.faction = db[1].ToUint32();
                    g.flags = db[2].ToUint32();

                    gs.entry = db[3].ToUint32();
                    gs.faction = db[4].ToUint32();
                    gs.flags = db[5].ToUint32();

                    go.Add(g);
                    gos.Add(gs);
                }
            }
            return CompareGameObject();
        }

        static bool CompareGameObject()
        {
            using (StreamWriter writer = new StreamWriter("gameobject_template.sql", true))
            {
                uint entry = 0;
                int counts = modelS.Count;

                for (int i = 0; i < counts; ++i)
                {
                    GameobjectTemplate g = go[i];
                    GameobjectTemplate gs = gos[i];
                    entry = g.entry;

                    if (!object.Equals(g.faction, gs.faction))
                        Query("faction", g.faction, entry, writer);

                    if (!object.Equals(g.flags, gs.flags))
                        Query("flags", g.flags, entry, writer);

                    if (i == (counts - 1))
                        Console.WriteLine("==========|| {0,-11}|| {1,-13}|| {2,-21}|| {3}||", counts, BadQuery, table, ((float)BadQuery / (float)counts) * 100);
                }
            }

            return false;
        }

        static void CrashReport(string message)
        {
            Console.WriteLine("<<ERROR>>");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void Query(string field0, object value, uint entry, StreamWriter writer)
        {
            writer.WriteLine(string.Format(NumberFormatInfo.InvariantInfo, "UPDATE `{0}` SET `{1}` = '{2}' WHERE {3} = {4};", table, field0, value, field, entry)); writer.Flush();
            ++BadQuery;
        }

        static void Query(string field_1, string field_2, object value, uint entry, StreamWriter writer)
        {
            writer.WriteLine(string.Format(NumberFormatInfo.InvariantInfo, "UPDATE `{0}` SET `{1}` = '{2}', `{3}` = '{2}' WHERE {4} = {5};", table, field_1, value, field_2, field, entry)); writer.Flush();
            ++BadQuery;
        }

        static bool ConnectionIsOpen
        {
            get
            {
                try
                {
                    connection.Open();
                    connection.Close();
                    connection.Open();
                    return true;
                }
                catch
                {
                    CrashReport("Check your MySQL server");
                    return false;
                }
            }
        }
    }
}
