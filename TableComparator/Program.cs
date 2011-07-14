using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;
using TableComparator.TableStructure;

namespace TableComparator
{
    internal class Program
    {
        private MySqlCommand _command;
        private MySqlConnection _connection;

        private readonly List<CreatureTemplate> _creatures = new List<CreatureTemplate>();
        private readonly List<CreatureTemplate> _sniffCreatures = new List<CreatureTemplate>();

        private readonly List<ModelInfo> _models = new List<ModelInfo>();
        private readonly List<ModelInfo> _sniffModels = new List<ModelInfo>();

        private readonly List<GameobjectTemplate> _gameObjects = new List<GameobjectTemplate>();
        private readonly List<GameobjectTemplate> _sniffGameObjects = new List<GameobjectTemplate>();

        private readonly List<EquipmentTemplate> _equipments = new List<EquipmentTemplate>();
        private readonly List<EquipmentTemplate> _sniffEquipments = new List<EquipmentTemplate>();

        private static void Main()
        {
            Program p = new Program();
            p.Start();
        }

        private void Start()
        {
            using (StreamReader reader = new StreamReader("config.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (Config));
                Config config = (Config) serializer.Deserialize(reader);

                string connectionInfo =
                    string.Format(
                        "host={0};port='{1}';database='{2}';UserName='{3}';Password='{4}';Connection Timeout='{5}'",
                        config.Host, config.Port, config.DataBase, config.UserName, config.Password,
                        config.ConnectionTimeOut);

                _connection = new MySqlConnection(connectionInfo);
            }

            Console.Title = "Table Comparator v1.1";

            if (!ConnectionIsOpen)
                return;

            Generic.FieldName = "entry";
            Console.WriteLine("==========||====================================================||=========||");
            Console.WriteLine("Statistics|| Templates: || Bad fields:  || Table:               || %       ||");

            SelectCreature();
            SelectGameObject();
            SelectEquip();
            SelectModel();
            Console.WriteLine("==========||====================================================||=========||");
        }

        private bool SelectCreature()
        {
            Generic.TableName = "creature_template";
            Generic.BadQuery = 0;
            const string query =
                "SELECT creature_template.entry, creature_template.speed_run, creature_template.speed_walk, creature_template.faction_A, creature_template.dynamicflags, creature_template.unit_flags, creature_template.rangeattacktime, creature_template.baseattacktime, creature_template.scale, creature_template.unit_class, creature_template.mindmg, creature_template.maxdmg, creature_template.attackpower, creature_template.rangedattackpower, creature_template.dmg_multiplier, creature_template.VehicleId, creature_template_sniff.entry, creature_template_sniff.speed_run, creature_template_sniff.speed_walk, creature_template_sniff.faction_A, creature_template_sniff.dynamicflags, creature_template_sniff.unit_flags, creature_template_sniff.rangeattacktime, creature_template_sniff.baseattacktime, creature_template_sniff.scale, creature_template_sniff.unit_class, creature_template_sniff.mindmg, creature_template_sniff.maxdmg, creature_template_sniff.attackpower, creature_template_sniff.rangedattackpower, creature_template_sniff.dmg_multiplier, creature_template_sniff.VehicleId FROM creature_template INNER JOIN creature_template_sniff ON creature_template.`entry` = creature_template_sniff.`entry` ORDER BY creature_template.entry";
            _command = new MySqlCommand(query, _connection);
            using (MySqlDataReader db = _command.ExecuteReader())
            {
                while (db.Read())
                {
                    CreatureTemplate creature = new CreatureTemplate();
                    CreatureTemplate sniffCreature = new CreatureTemplate();

                    creature.entry = db[0].ToUint32();
                    creature.speed_run = db[1].ToFloat();
                    creature.speed_walk = db[2].ToFloat();
                    creature.faction = db[3].ToUint32();
                    creature.dynamicFlags = db[4].ToUint32();
                    creature.unit_flag = db[5].ToUint32();
                    creature.rangeAttackTime = db[6].ToUint32();
                    creature.baseAttackTime = db[7].ToUint32();
                    creature.scale = db[8].ToFloat();
                    creature.unit_class = db[9].ToUint32();
                    creature.minDamage = db[10].ToFloat();
                    creature.maxDamage = db[11].ToFloat();
                    creature.attackPower = db[12].ToUint32();
                    creature.rangeAttackPower = db[13].ToUint32();
                    creature.dmgMultiplier = db[14].ToUint32();
                    creature.vehicleId = db[15].ToUInt16();

                    sniffCreature.entry = db[16].ToUint32();
                    sniffCreature.speed_run = db[17].ToFloat();
                    sniffCreature.speed_walk = db[18].ToFloat();
                    sniffCreature.faction = db[19].ToUint32();
                    sniffCreature.dynamicFlags = db[20].ToUint32();
                    sniffCreature.unit_flag = db[21].ToUint32();
                    sniffCreature.rangeAttackTime = db[22].ToUint32();
                    sniffCreature.baseAttackTime = db[23].ToUint32();
                    sniffCreature.scale = db[24].ToFloat();
                    sniffCreature.unit_class = db[25].ToUint32();
                    sniffCreature.minDamage = db[26].ToFloat();
                    sniffCreature.maxDamage = db[27].ToFloat();
                    sniffCreature.attackPower = db[28].ToUint32();
                    sniffCreature.rangeAttackPower = db[29].ToUint32();
                    sniffCreature.dmgMultiplier = db[30].ToUint32();
                    sniffCreature.vehicleId = db[31].ToUInt16();

                    _creatures.Add(creature);
                    _sniffCreatures.Add(sniffCreature);
                }
            }
            return CreatureCompare();
        }

        private bool CreatureCompare()
        {
            using (StreamWriter writer = new StreamWriter("creature_template.sql", true))
            {
                writer.WriteLine(string.Format("-- Dump of {0}", DateTime.Now));

                int counts = _sniffCreatures.Count;

                for (int i = 0; i < counts; ++i)
                {
                    CreatureTemplate creature = _creatures[i];
                    CreatureTemplate sniffCreature = _sniffCreatures[i];
                    uint entry = sniffCreature.entry;

                    if (!Equals(creature.speed_run, sniffCreature.speed_run))
                        writer.WriteQuery("speed_run", sniffCreature.speed_run, entry);

                    if (!Equals(creature.speed_walk, sniffCreature.speed_walk))
                        writer.WriteQuery("speed_walk", sniffCreature.speed_walk, entry);

                    if (!Equals(creature.vehicleId, sniffCreature.vehicleId))
                        writer.WriteQuery("vehicle_id", sniffCreature.vehicleId, entry);

                    if (!Equals(creature.unit_class, sniffCreature.unit_class))
                        writer.WriteQuery("unit_class", sniffCreature.unit_class, entry);

                    if (!Equals(creature.unit_flag, sniffCreature.unit_flag))
                        writer.WriteQuery("unit_flags", sniffCreature.unit_flag, entry);

                    if (!Equals(creature.faction, sniffCreature.faction))
                        writer.WriteQuery("faction_A", "faction_H", sniffCreature.faction, entry);

                    if (!Equals(creature.dynamicFlags, sniffCreature.dynamicFlags))
                        writer.WriteQuery("dynamicflags", sniffCreature.dynamicFlags, entry);

                    if (!Equals(creature.scale, sniffCreature.scale))
                        writer.WriteQuery("scale", sniffCreature.scale, entry);

                    if (!Equals(creature.maxDamage, sniffCreature.maxDamage))
                        writer.WriteQuery("maxdmg", sniffCreature.maxDamage, entry);

                    if (!Equals(creature.minDamage, sniffCreature.minDamage))
                        writer.WriteQuery("mindmg", sniffCreature.minDamage, entry);

                    if (!Equals(creature.dmgMultiplier, sniffCreature.dmgMultiplier))
                        writer.WriteQuery("dmg_multiplier", sniffCreature.dmgMultiplier, entry);

                    if (!Equals(creature.attackPower, sniffCreature.attackPower))
                        writer.WriteQuery("attackpower", sniffCreature.attackPower, entry);

                    if (!Equals(creature.rangeAttackPower, sniffCreature.rangeAttackPower))
                        writer.WriteQuery("rangedattackpower", sniffCreature.rangeAttackPower, entry);

                    if (!Equals(creature.rangeAttackTime, sniffCreature.rangeAttackTime))
                        writer.WriteQuery("rangeattacktime", sniffCreature.rangeAttackTime, entry);

                    if (!Equals(creature.baseAttackTime, sniffCreature.baseAttackTime))
                        writer.WriteQuery("baseattacktime", sniffCreature.baseAttackTime, entry);

                    if (i == (counts - 1))
                        Console.WriteLine("==========|| {0,-11}|| {1,-13}|| {2,-21}|| {3}||", counts, Generic.BadQuery,
                                          Generic.TableName, (Generic.BadQuery/(float) counts)*100);
                }

                writer.Flush();
            }

            return false;
        }

        private bool SelectModel()
        {
            Generic.TableName = "creature_model_info";
            Generic.FieldName = "modelId";
            Generic.BadQuery = 0;
            const string query =
                "SELECT creature_model_info.`modelid`, creature_model_info.`bounding_radius`, creature_model_info.`combat_reach`, creature_model_info.`gender`, creature_model_info_sniff.`modelid`, creature_model_info_sniff.`bounding_radius`, creature_model_info_sniff.`combat_reach`, creature_model_info_sniff.`gender` FROM creature_model_info INNER JOIN creature_model_info_sniff ON creature_model_info.`modelid` = creature_model_info_sniff.`modelid` ORDER BY creature_model_info.`modelid`";
            _command = new MySqlCommand(query, _connection);

            using (MySqlDataReader db = _command.ExecuteReader())
            {
                while (db.Read())
                {
                    ModelInfo model = new ModelInfo();
                    ModelInfo sniffModel = new ModelInfo();

                    model.modelid = db[0].ToUint32();
                    model.bounding_radius = db[1].ToFloat();
                    model.combat_reach = db[2].ToFloat();
                    model.gender = db[3].ToSByte();

                    sniffModel.modelid = db[4].ToUint32();
                    sniffModel.bounding_radius = db[5].ToFloat();
                    sniffModel.combat_reach = db[6].ToFloat();
                    sniffModel.gender = db[7].ToSByte();

                    _models.Add(model);
                    _sniffModels.Add(sniffModel);
                }
            }
            return CompareModel();
        }

        private bool CompareModel()
        {
            using (StreamWriter writer = new StreamWriter("creature_model_info.sql", true))
            {
                writer.WriteLine(string.Format("-- Dump of {0}", DateTime.Now));

                int counts = _sniffModels.Count;

                for (int i = 0; i < counts; ++i)
                {
                    ModelInfo model = _models[i];
                    ModelInfo sniffModel = _sniffModels[i];
                    uint modelId = model.modelid;

                    if (!Equals(model.bounding_radius, sniffModel.bounding_radius))
                        writer.WriteQuery("bounding_radius", sniffModel.bounding_radius, modelId);

                    if (!Equals(model.combat_reach, sniffModel.combat_reach))
                        writer.WriteQuery("combat_reach", sniffModel.combat_reach, modelId);

                    if (!Equals(model.gender, sniffModel.gender))
                        writer.WriteQuery("gender", sniffModel.gender, modelId);

                    if (i == (counts - 1))
                        Console.WriteLine("==========|| {0,-11}|| {1,-13}|| {2,-21}|| {3}||", counts, Generic.BadQuery,
                                          Generic.TableName, (Generic.BadQuery/(float) counts)*100);
                }

                writer.Flush();
            }

            return false;
        }

        private bool SelectGameObject()
        {
            Generic.TableName = "gameobject_template";
            Generic.BadQuery = 0;
            const string query =
                "SELECT gameobject_template.`entry`, gameobject_template.`faction`, gameobject_template.`flags`, gameobject_template_sniff.`entry`, gameobject_template_sniff.`faction`, gameobject_template_sniff.`flags` FROM gameobject_template INNER JOIN gameobject_template_sniff ON gameobject_template.`entry` = gameobject_template_sniff.`entry` ORDER BY gameobject_template.`entry`";
            _command = new MySqlCommand(query, _connection);

            using (MySqlDataReader db = _command.ExecuteReader())
            {
                while (db.Read())
                {
                    GameobjectTemplate gameObject = new GameobjectTemplate();
                    GameobjectTemplate sniffGameObject = new GameobjectTemplate();

                    gameObject.entry = db[0].ToUint32();
                    gameObject.faction = db[1].ToUint32();
                    gameObject.flags = db[2].ToUint32();

                    sniffGameObject.entry = db[3].ToUint32();
                    sniffGameObject.faction = db[4].ToUint32();
                    sniffGameObject.flags = db[5].ToUint32();

                    _gameObjects.Add(gameObject);
                    _sniffGameObjects.Add(sniffGameObject);
                }
            }
            return CompareGameObject();
        }

        private bool CompareGameObject()
        {
            using (StreamWriter writer = new StreamWriter("gameobject_template.sql", true))
            {
                writer.WriteLine(string.Format("-- Dump of {0}", DateTime.Now));

                int counts = _sniffGameObjects.Count;

                for (int i = 0; i < counts; ++i)
                {
                    GameobjectTemplate gameObject = _gameObjects[i];
                    GameobjectTemplate sniffGameObject = _sniffGameObjects[i];
                    uint entry = gameObject.entry;

                    if (!Equals(gameObject.faction, sniffGameObject.faction))
                        writer.WriteQuery("faction", gameObject.faction, entry);

                    if (!Equals(gameObject.flags, sniffGameObject.flags))
                        writer.WriteQuery("flags", gameObject.flags, entry);

                    if (i == (counts - 1))
                        Console.WriteLine("==========|| {0,-11}|| {1,-13}|| {2,-21}|| {3}||", counts, Generic.BadQuery,
                                          Generic.TableName, (Generic.BadQuery/(float) counts)*100);
                }

                writer.Flush();
            }

            return false;
        }

        private bool SelectEquip()
        {
            Generic.TableName = "creature_equip_template";
            Generic.BadQuery = 0;
            const string query =
                "SELECT creature_equip_template.`entry`, creature_equip_template.`equipEntry1`, creature_equip_template.`equipEntry2`, creature_equip_template.`equipEntry3`, creature_equip_template_sniff.`entry`, creature_equip_template_sniff.`equipEntry1`, creature_equip_template_sniff.`equipEntry2`, creature_equip_template_sniff.`equipEntry3` FROM creature_equip_template INNER JOIN creature_equip_template_sniff ON creature_equip_template.`entry` = creature_equip_template_sniff.`entry` ORDER BY creature_equip_template.`entry`";
            _command = new MySqlCommand(query, _connection);

            using (MySqlDataReader db = _command.ExecuteReader())
            {
                while (db.Read())
                {
                    EquipmentTemplate equipment = new EquipmentTemplate();
                    EquipmentTemplate sniffEquipment = new EquipmentTemplate();

                    equipment.entry = db[0].ToUint32();
                    equipment.equipEntry = new uint[3];
                    for (int i = 0; i < 3; ++i)
                        equipment.equipEntry[i] = db[1 + i].ToUint32();

                    sniffEquipment.entry = db[4].ToUint32();
                    sniffEquipment.equipEntry = new uint[3];
                    for (int i = 0; i < 3; ++i)
                        sniffEquipment.equipEntry[i] = db[4 + i].ToUint32();

                    _equipments.Add(equipment);
                    _sniffEquipments.Add(sniffEquipment);
                }
            }
            return CompareEquip();
        }

        private bool CompareEquip()
        {
            using (StreamWriter writer = new StreamWriter("creature_equip_template.sql", true))
            {
                writer.WriteLine(string.Format("-- Dump of {0}", DateTime.Now));

                int counts = _equipments.Count;

                for (int i = 0; i < counts; ++i)
                {
                    EquipmentTemplate equipment = _equipments[i];
                    EquipmentTemplate sniffEquipment = _sniffEquipments[i];

                    for (int j = 0; j < 3; ++j)
                    {
                        if (!Equals(equipment.equipEntry[j], sniffEquipment.equipEntry[j]))
                            writer.WriteQuery("equipEntry" + (j + 1), equipment.equipEntry[j], equipment.entry);
                    }

                    if (i == (counts - 1))
                        Console.WriteLine("==========|| {0,-11}|| {1,-13}|| {2,-21}|| {3}||", counts, Generic.BadQuery,
                                          Generic.TableName, (Generic.BadQuery/(float) counts)*100);
                }

                writer.Flush();
            }

            return false;
        }

        private bool ConnectionIsOpen
        {
            get
            {
                try
                {
                    _connection.Open();
                    _connection.Close();
                    _connection.Open();
                    return true;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Check your MySQL server");
                    return false;
                }
            }
        }
    }
}