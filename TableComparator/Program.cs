using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TableComparator.TableStructure;
using System.Globalization;
using System.IO;

namespace TableComparator
{
    class Program
    {
        const string connectionInfo = ("host=127.0.0.1;port='3306';database='mangos';UserName='mangos';Password='mangos';Connection Timeout='120000';");
        static MySqlCommand command;
        static MySqlConnection connection = new MySqlConnection(connectionInfo);

        static List<Creature> creature = new List<Creature>(); //normal data
        static List<Creature> creatureS = new List<Creature>(); //data from sniff

        static List<ModelInfo> model = new List<ModelInfo>();
        static List<ModelInfo> modelS = new List<ModelInfo>();

        static uint BadQuery;
        static string table;
        static string field;

        static void Main(string[] args)
        {
            Console.Title = "Table Comparator v1.0";
            AppDomain.CurrentDomain.UnhandledException +=
                (o, e) => CrashReport(e.ExceptionObject.ToString());

            if (!ConnectionIsOpen)
                return;

            BadQuery = 0;
            SelectCreature();
            SelectModel();
        }

        static bool SelectCreature()
        {
            Console.WriteLine("Select creature_template data...");

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

                    cr.entry = db[0].ToUint();
                    cr.speed_run = db[1].ToFloat();
                    cr.speed_walk = db[2].ToFloat();
                    cr.faction = db[3].ToUint();
                    cr.dynamicFlags = db[4].ToUint();
                    cr.unit_flag = db[5].ToUint();
                    cr.rangeAttackTime = db[6].ToUint();
                    cr.baseAttackTime = db[7].ToUint();
                    cr.scale = db[8].ToFloat();
                    cr.unit_class = db[9].ToUint();
                    cr.minDamage = db[10].ToFloat();
                    cr.maxDamage = db[11].ToFloat();
                    cr.attackPower = db[12].ToUint();
                    cr.rangeAttackPower = db[13].ToUint();
                    cr.dmgMultiplier = db[14].ToUint();
                    cr.vehicleId = db[15].ToUint16();

                    crs.entry = db[16].ToUint();
                    crs.speed_run = db[17].ToFloat();
                    crs.speed_walk = db[18].ToFloat();
                    crs.faction = db[19].ToUint();
                    crs.dynamicFlags = db[20].ToUint();
                    crs.unit_flag = db[21].ToUint();
                    crs.rangeAttackTime = db[22].ToUint();
                    crs.baseAttackTime = db[23].ToUint();
                    crs.scale = db[24].ToFloat();
                    crs.unit_class = db[25].ToUint();
                    crs.minDamage = db[26].ToFloat();
                    crs.maxDamage = db[27].ToFloat();
                    crs.attackPower = db[28].ToUint();
                    crs.rangeAttackPower = db[29].ToUint();
                    crs.dmgMultiplier = db[30].ToUint();
                    crs.vehicleId = db[31].ToUint16();

                    creature.Add(cr);
                    creatureS.Add(crs);
                }
            }
            return CreatureCompare(); ;
        }

        static bool CreatureCompare()
        {
            Console.WriteLine("Compare creature_template data...");

            using (StreamWriter writer = new StreamWriter("creature_template.sql", true))
            {
                uint entry = 0;
                int counts = creatureS.Count;

                for (int i = 0; i < counts; ++i)
                {
                    entry = creatureS[i].entry;
                    if (!object.Equals(creature[i].speed_run, creatureS[i].speed_run))
                        Query("speed_run", creatureS[i].speed_run, entry, writer);

                    if (!object.Equals(creature[i].speed_walk, creatureS[i].speed_walk))
                        Query("speed_walk", creatureS[i].speed_walk, entry, writer);

                    if (!object.Equals(creature[i].vehicleId, creatureS[i].vehicleId))
                        Query("vehicle_id", creatureS[i].vehicleId, entry, writer);

                    if (!object.Equals(creature[i].unit_class, creatureS[i].unit_class))
                        Query("unit_class", creatureS[i].unit_class, entry, writer);

                    if (!object.Equals(creature[i].unit_flag, creatureS[i].unit_flag))
                        Query("unit_flags", creatureS[i].unit_flag, entry, writer);

                    if (!object.Equals(creature[i].faction, creatureS[i].faction))
                        Query("faction_A", "faction_H", creatureS[i].faction, entry, writer);

                    if (!object.Equals(creature[i].dynamicFlags, creatureS[i].dynamicFlags))
                        Query("dynamicflags", creatureS[i].dynamicFlags, entry, writer);

                    if (!object.Equals(creature[i].scale, creatureS[i].scale))
                        Query("scale", creatureS[i].scale, entry, writer);

                    if (!object.Equals(creature[i].maxDamage, creatureS[i].maxDamage))
                        Query("maxdmg", creatureS[i].maxDamage, entry, writer);

                    if (!object.Equals(creature[i].minDamage, creatureS[i].minDamage))
                        Query("mindmg", creatureS[i].minDamage, entry, writer);

                    if (!object.Equals(creature[i].dmgMultiplier, creatureS[i].dmgMultiplier))
                        Query("dmg_multiplier", creatureS[i].dmgMultiplier, entry, writer);

                    if (!object.Equals(creature[i].attackPower, creatureS[i].attackPower))
                        Query("attackpower", creatureS[i].attackPower, entry, writer);

                    if (!object.Equals(creature[i].rangeAttackPower, creatureS[i].rangeAttackPower))
                        Query("rangedattackpower", creatureS[i].rangeAttackPower, entry, writer);

                    if (!object.Equals(creature[i].rangeAttackTime, creatureS[i].rangeAttackTime))
                        Query("rangeattacktime", creatureS[i].rangeAttackTime, entry, writer);

                    if (!object.Equals(creature[i].baseAttackTime, creatureS[i].baseAttackTime))
                        Query("baseattacktime", creatureS[i].baseAttackTime, entry, writer);

                    if (i == (counts - 1))
                        Statistics(counts);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done!");
            Console.ResetColor();

            return false;
        }

        static bool SelectModel()
        {
            Console.WriteLine("Select creature_model_info data...");

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

                    mod.modelid = db[0].ToUint();
                    mod.bounding_radius = db[1].ToFloat();
                    mod.combat_reach = db[2].ToFloat();
                    mod.gender = db[3].ToSByte();

                    modS.modelid = db[4].ToUint();
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
            Console.WriteLine("Compare creature_model_info data...");

            using (StreamWriter writer = new StreamWriter("creature_model_info.sql", true))
            {
                uint modelId = 0;
                int counts = modelS.Count;
                for (int i = 0; i < counts; ++i)
                {
                    modelId = modelS[i].modelid;
                    if (!object.Equals(model[i].bounding_radius, modelS[i].bounding_radius))
                        Query("bounding_radius", modelS[i].bounding_radius, modelId, writer);

                    if (!object.Equals(model[i].combat_reach, modelS[i].combat_reach))
                        Query("combat_reach", modelS[i].combat_reach, modelId, writer);

                    if (!object.Equals(model[i].gender, modelS[i].gender))
                        Query("gender", modelS[i].gender, modelId, writer);

                    if (i == (counts - 1))
                        Statistics(counts);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done!");
            Console.ResetColor();

            return false;
        }

        static void CrashReport(string message)
        {
            Console.WriteLine("<<ERROR>>");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void Statistics(int Count)
        {
            Console.WriteLine("==========||====================================================||");
            Console.WriteLine("Statistics|| Templates: || Bad fields: || table name:        ||");
            Console.WriteLine("==========|| {0,-12}| {1,-16}| {2,-12}||", Count, BadQuery, table);
            Console.WriteLine("==========||====================================================||");
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
