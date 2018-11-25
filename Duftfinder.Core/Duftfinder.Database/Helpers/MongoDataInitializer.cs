using System;
using System.Reflection;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Duftfinder.Database.Helpers
{
    /// <summary>
    /// Initializes all the predefined values in MongoDB.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class MongoDataInitializer
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MongoContext _mongoContext;

        // FindOptions are used when finding a document in MongoDB. 
        // CollationStrength.Primary is used in order to perform comparison invariant of case.
        private readonly FindOptions _findOptions = new FindOptions { Collation = new Collation(Constants.de, strength: CollationStrength.Primary) };

        public MongoDataInitializer(MongoContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        /// <summary>
        /// Initializes all collections that require default values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        public async Task InitSubstancesAndCategories()
        {
            // Initialize Substances and Molecules
            await InitSubstanceCollection(SubstanceValue.AromaticAlcohol);
            await InitSubstanceCollection(SubstanceValue.AromaticAldehyde);
            await InitSubstanceCollection(SubstanceValue.AromaticEster);
            await InitSubstanceCollection(SubstanceValue.Cumarin);
            await InitSubstanceCollection(SubstanceValue.Ester);
            await InitSubstanceCollection(SubstanceValue.Ketone);
            await InitSubstanceCollection(SubstanceValue.Monoterpene);
            await InitSubstanceCollection(SubstanceValue.Monoterpenole);
            await InitSubstanceCollection(SubstanceValue.Phenole);
            await InitSubstanceCollection(SubstanceValue.Aldehyde);
            await InitSubstanceCollection(SubstanceValue.Oxide);
            await InitSubstanceCollection(SubstanceValue.Phenylpropanderivate);
            await InitSubstanceCollection(SubstanceValue.Sesquiterpene);
            await InitSubstanceCollection(SubstanceValue.SesquiterpenoleDiterpenole);

            // Initialize Categories
            await InitCategoryCollection(CategoryValue.Pain);
            await InitCategoryCollection(CategoryValue.Muscle);
            await InitCategoryCollection(CategoryValue.SkinAndScars);
            await InitCategoryCollection(CategoryValue.Tissue);
            await InitCategoryCollection(CategoryValue.BloodCirculation);
            await InitCategoryCollection(CategoryValue.NervousSystemGeneral);
            await InitCategoryCollection(CategoryValue.NervousSystemNeurotransmitter);
            await InitCategoryCollection(CategoryValue.EntericNervousSystem);
            await InitCategoryCollection(CategoryValue.EndocrineHormones);
            await InitCategoryCollection(CategoryValue.ImmuneSystem);
            await InitCategoryCollection(CategoryValue.Inflammation);
            await InitCategoryCollection(CategoryValue.IntegrationYoungerSelf);
            await InitCategoryCollection(CategoryValue.OrderInCircle);
            await InitCategoryCollection(CategoryValue.AccessToInnerStrength);
            await InitCategoryCollection(CategoryValue.BlockageDissolving);
            await InitCategoryCollection(CategoryValue.AchievingGoalsAttention);
            await InitCategoryCollection(CategoryValue.AchievingGoalsActivationLevel);
            await InitCategoryCollection(CategoryValue.AchievingGoalsNerves);
            await InitCategoryCollection(CategoryValue.MeridiansAttackYinYang);
        }

        public async Task InitEssentialOils()
        {
            // Initialize Essential Oils
            await InitEssentialOilCollection("Angelika", "Angelica archangelica", EssentialOilType.Oil);
            await InitEssentialOilCollection("Backhousia", "Backhousia citriodora", EssentialOilType.Oil);
            await InitEssentialOilCollection("Basilikum, grossblättrig", "Ocimum basilicum ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Benzoe", "Styrax tonkinensis", EssentialOilType.Oil);
            await InitEssentialOilCollection("Bergamotte", "Citrus bergamia", EssentialOilType.Oil);
            await InitEssentialOilCollection("Bergamotte-Minze", "Mentha citrata", EssentialOilType.Oil);
            await InitEssentialOilCollection("Cajeput", "Melaleuca leucadendron ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Cistrose", "Cistus ladaniferus", EssentialOilType.Oil);
            await InitEssentialOilCollection("Davana", "Artemisia pallens", EssentialOilType.Oil);
            await InitEssentialOilCollection("Eukalyptus citriodora", "Eucalyptus citriodora", EssentialOilType.Oil);
            await InitEssentialOilCollection("Eukalyptus staigeriana", "Eucalyptus staigeriana", EssentialOilType.Oil);
            await InitEssentialOilCollection("Fichte, sibirisch", "Abies sibirica", EssentialOilType.Oil);
            await InitEssentialOilCollection("Gingergras", "Cymbopogon martinii var. sofia", EssentialOilType.Oil);
            await InitEssentialOilCollection("Grapefruit", "Citrus x paradisi", EssentialOilType.Oil);
            await InitEssentialOilCollection("Ho-Blätter ", "Cinnamomum camphora linalolifera", EssentialOilType.Oil);
            await InitEssentialOilCollection("Immortelle", "Helichrysum italicum", EssentialOilType.Oil);
            await InitEssentialOilCollection("Indianernessel", "Monarda fistulosa L. Ct. geraniol", EssentialOilType.Oil);
            await InitEssentialOilCollection("Ingwer", "Zingiber officinalis  ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Jasmin, 10% in Jojobaöl", "Jasminum grandiflorum  ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Kamille, blau", "Matricaria chamomilla", EssentialOilType.Oil);
            await InitEssentialOilCollection("Kamille, römisch", "Chamaemelum nobile ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Karottensamen", "Daucus carota", EssentialOilType.Oil);
            await InitEssentialOilCollection("Lavendel ", "Lavendula angustifolia (1400 m)", EssentialOilType.Oil);
            await InitEssentialOilCollection("Limette", "Citrus aurantifolia", EssentialOilType.Oil);
            await InitEssentialOilCollection("Linaloebeeren", "Bursera delpechiana", EssentialOilType.Oil);
            await InitEssentialOilCollection("Litsea  ", "Litsea cubeba", EssentialOilType.Oil);
            await InitEssentialOilCollection("Lorbeer", "Laurus nobilis ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Majoran, süss", "Origanum majorana L.", EssentialOilType.Oil);
            await InitEssentialOilCollection("Mandarine, rot", "Citrus reticulata", EssentialOilType.Oil);
            await InitEssentialOilCollection("Manuka", "Leptosperumum scoparium", EssentialOilType.Oil);
            await InitEssentialOilCollection("Melisse", "Melissa officinalis ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Muskatellersalbei", "Salvia sclarea", EssentialOilType.Oil);
            await InitEssentialOilCollection("Myrrhe", "Commiphora myrrha N.", EssentialOilType.Oil);
            await InitEssentialOilCollection("Nanaminze", "Mentha viridis L. Nana", EssentialOilType.Oil);
            await InitEssentialOilCollection("Neroli", "Citrus x. aurantium (Blüten)", EssentialOilType.Oil);
            await InitEssentialOilCollection("Niaouli", "Melaleuca viridiflora ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Orange, bitter", "Citrus x. aurantium (Schale)", EssentialOilType.Oil);
            await InitEssentialOilCollection("Palmarosa", "Cymbopogon martinii Var. motia", EssentialOilType.Oil);
            await InitEssentialOilCollection("Patchouli", "Pogostermon cablin  ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Petitgrain Mandarine", "Citrus reticulata (Blätter)", EssentialOilType.Oil);
            await InitEssentialOilCollection("Pfeffer, schwarz", "Piper nigrum", EssentialOilType.Oil);
            await InitEssentialOilCollection("Pfefferminze", "Mentha x piperita", EssentialOilType.Oil);
            await InitEssentialOilCollection("Ravintsara", "Cinnamomum camphora Ct. cineol", EssentialOilType.Oil);
            await InitEssentialOilCollection("Rose 10% in Jojobaöl", "Rosa damascena", EssentialOilType.Oil);
            await InitEssentialOilCollection("Rosengeranium", "Pelargonium x asperum ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Rosmarin Ct. cineol", "Rosmarinus officinalis Ct. cineol", EssentialOilType.Oil);
            await InitEssentialOilCollection("Rosmarin Ct. verbenon", "Rosmarinus officinalis Ct. verbenon", EssentialOilType.Oil);
            await InitEssentialOilCollection("Sandelholz", "Santalum austrocaledonicum", EssentialOilType.Oil);
            await InitEssentialOilCollection("Thymian Ct. linalool", "Thymus vulgaris Ct. linalool", EssentialOilType.Oil);
            await InitEssentialOilCollection("Tonkabohne", "Dipteryx odorata ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Vanille", "Vanilla planifolia", EssentialOilType.Oil);
            await InitEssentialOilCollection("Vetiver", "Vetiveria zizanoides", EssentialOilType.Oil);
            await InitEssentialOilCollection("Wacholder", "Juniperus communis L.", EssentialOilType.Oil);
            await InitEssentialOilCollection("Weihrauch", "Boswellia carterii B.", EssentialOilType.Oil);
            await InitEssentialOilCollection("Wintergrün", "Gaultheria fragrantissima  ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Ylang-Ylang, extra", "Cananga odorata ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Ysop-Berg", "Hyssopus officinalis L. var montana", EssentialOilType.Oil);
            await InitEssentialOilCollection("Zeder-Atlas", "Cedrus atlantica  ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Zitrone ", "Citrus limon ", EssentialOilType.Oil);
            await InitEssentialOilCollection("Zypresse", "Cupressus sempervirens", EssentialOilType.Oil);
        }

        public async Task InitEffects()
        {
            // Initialize Effects
            await InitEffectCollection("Schmerzlindernd, - stillend", CategoryValue.Pain);
            await InitEffectCollection("lokalanästhetisch", CategoryValue.Pain);
            await InitEffectCollection("Schmerzlindernd: Gelenke", CategoryValue.Pain);
            await InitEffectCollection("Schmerzlindernd: Muskeln", CategoryValue.Pain);
            await InitEffectCollection("Schmerzlindernd: Kopf", CategoryValue.Pain);
            await InitEffectCollection("Körpermuskeln entkrampfend, entspannend", CategoryValue.Muscle);
            await InitEffectCollection("Glatte Muskeln entkrampfend (spasmolytisch)", CategoryValue.Muscle);
            await InitEffectCollection("Bronchien entkrampfend (spasmolytisch)", CategoryValue.Muscle);
            await InitEffectCollection("Muskelspannung fördernd (tonisierend)", CategoryValue.Muscle);
            await InitEffectCollection("Hautpflegend, schützend", CategoryValue.SkinAndScars);
            await InitEffectCollection("Hautstoffwechsel anregend", CategoryValue.SkinAndScars);
            await InitEffectCollection("Haut regenerierend", CategoryValue.SkinAndScars);
            await InitEffectCollection("Hautzellerneuernd (epithelisierend)", CategoryValue.SkinAndScars);
            await InitEffectCollection("Juckreiz stillend", CategoryValue.SkinAndScars);
            await InitEffectCollection("Wundheilung fördernd", CategoryValue.SkinAndScars);
            await InitEffectCollection("Narben pflegend (frische)", CategoryValue.SkinAndScars);
            await InitEffectCollection("Narben pflegend (alte)", CategoryValue.SkinAndScars);
            await InitEffectCollection("Narbenberührung erleichternd", CategoryValue.SkinAndScars);
            await InitEffectCollection("Durchblutungsfördernd", CategoryValue.Tissue);
            await InitEffectCollection("Adstingierend", CategoryValue.Tissue);
            await InitEffectCollection("Entstauend (venös oder lymphatisch)", CategoryValue.Tissue);
            await InitEffectCollection("Lymphfluss anregend", CategoryValue.Tissue);
            await InitEffectCollection("Aquaretisch, diuretisch", CategoryValue.Tissue);
            await InitEffectCollection("Hämatomauflösend", CategoryValue.Tissue);
            await InitEffectCollection("Bindegewebsstabilisierend", CategoryValue.Tissue);
            await InitEffectCollection("Zellregeneriernd, -erneuernd", CategoryValue.Tissue);
            await InitEffectCollection("Kreislauf anregend, belebend", CategoryValue.BloodCirculation);
            await InitEffectCollection("Herz- und Kreislauf schützend (stärkend)", CategoryValue.BloodCirculation);
            await InitEffectCollection("Kontraktionskraft Herz reduzieren", CategoryValue.BloodCirculation);
            await InitEffectCollection("Kontraktilität erhöhen", CategoryValue.BloodCirculation);
            await InitEffectCollection("Venen tonisieren", CategoryValue.BloodCirculation);
            await InitEffectCollection("Autonomes Nervensystem aufgleichend", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("Parasympathikus aktivierend", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("Sympathikus aktivierend", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("Neuralgien beruhigend", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("Schlaffördernd ", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("langsame Hirnwellen aktivierend", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("angstlösend", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("antidepressiv", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("stimmungsaufhellend", CategoryValue.NervousSystemGeneral);
            await InitEffectCollection("Acetylcholin", CategoryValue.NervousSystemNeurotransmitter);
            await InitEffectCollection("Dopamin", CategoryValue.NervousSystemNeurotransmitter);
            await InitEffectCollection("GABA: Rezeptorenempfindlichkeit steigernd", CategoryValue.NervousSystemNeurotransmitter);
            await InitEffectCollection("Noradrenalin: Ausschüttung anregend", CategoryValue.NervousSystemNeurotransmitter);
            await InitEffectCollection("Serotonin (aktivierend Raphus nucleus)", CategoryValue.NervousSystemNeurotransmitter);
            await InitEffectCollection("Verdauung fördernd", CategoryValue.EntericNervousSystem);
            await InitEffectCollection("Entblähend (karminativ)", CategoryValue.EntericNervousSystem);
            await InitEffectCollection("Leberunterstützend", CategoryValue.EntericNervousSystem);
            await InitEffectCollection("reinigend, entgiftend", CategoryValue.EntericNervousSystem);
            await InitEffectCollection("Fördern der Gallensekretion (cholagog)", CategoryValue.EntericNervousSystem);
            await InitEffectCollection("Enterisches Nervensystem balancierend (Stress und Ängste)", CategoryValue.EntericNervousSystem);
            await InitEffectCollection("Thalamus: Enkephaline", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Thalamus: Endorphine (Glückshormone)", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Hypophyse: aphrodisierend", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Epiphyse (Melatonin)", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Hormonmodulierend: Stress (Katecholamine)", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Hormonmodulierend: Stereoide", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Hormonmodulierend: Geschlechtshormone", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Hormonähnlich: Östrogene", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Menstruationsregulierend", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Menstruationsregulierend (Hypermenorrhoe)", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Nebennierenrinde modulierend", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Kortisonähnlich wirkend", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Pankreas anregend", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Prostaglandinsynthese hemmend", CategoryValue.EndocrineHormones);
            await InitEffectCollection("Immunstimulierend, - stärkend, -ausgleichend", CategoryValue.ImmuneSystem);
            await InitEffectCollection("antiallergisch", CategoryValue.ImmuneSystem);
            await InitEffectCollection("antihistaminisch", CategoryValue.ImmuneSystem);
            await InitEffectCollection("antiseptisch, antiinfektiös", CategoryValue.ImmuneSystem);
            await InitEffectCollection("Entzündungshemmend", CategoryValue.Inflammation);
            await InitEffectCollection("Seelisch aufhellend, heilend", CategoryValue.IntegrationYoungerSelf);
            await InitEffectCollection("öffnend", CategoryValue.IntegrationYoungerSelf);
            await InitEffectCollection("tröstend, Geborgenheit vermittelnd, schützend", CategoryValue.IntegrationYoungerSelf);
            await InitEffectCollection("Beruhigend, besänftigend, aufrichtend", CategoryValue.IntegrationYoungerSelf);
            await InitEffectCollection("Klärend", CategoryValue.OrderInCircle);
            await InitEffectCollection("Vertrauen fördernd", CategoryValue.OrderInCircle);
            await InitEffectCollection("Strukturierend", CategoryValue.OrderInCircle);
            await InitEffectCollection("Harmonisierend", CategoryValue.OrderInCircle);
            await InitEffectCollection("Stärkend, Selbstvertrauen aufbauend", CategoryValue.AccessToInnerStrength);
            await InitEffectCollection("Stabilisierend, erdend", CategoryValue.AccessToInnerStrength);
            await InitEffectCollection("Aufbauend, erdend", CategoryValue.AccessToInnerStrength);
            await InitEffectCollection("Aufbauend, Durchhalten fördernd", CategoryValue.AccessToInnerStrength);
            await InitEffectCollection("Inspirierend", CategoryValue.BlockageDissolving);
            await InitEffectCollection("Kreativität fördernd", CategoryValue.BlockageDissolving);
            await InitEffectCollection("Emotional entkrampfend", CategoryValue.BlockageDissolving);
            await InitEffectCollection("Stress reduzierend", CategoryValue.BlockageDissolving);
            await InitEffectCollection("Besänftigend, aufmunternd, Zuversicht entwickeln", CategoryValue.BlockageDissolving);
            await InitEffectCollection("Gedächtnisstärkend", CategoryValue.AchievingGoalsAttention);
            await InitEffectCollection("Konzentrationsfördernd", CategoryValue.AchievingGoalsAttention);
            await InitEffectCollection("Erfrischend", CategoryValue.AchievingGoalsAttention);
            await InitEffectCollection("Aktivierend, belebend", CategoryValue.AchievingGoalsActivationLevel);
            await InitEffectCollection("Allgemein entspannend", CategoryValue.AchievingGoalsActivationLevel);
            await InitEffectCollection("Nerven beruhigend", CategoryValue.AchievingGoalsNerves);
            await InitEffectCollection("Nerven ausgleichend", CategoryValue.AchievingGoalsNerves);
            await InitEffectCollection("Nerven stärkend", CategoryValue.AchievingGoalsNerves);
            await InitEffectCollection("Yin stimulierend", CategoryValue.MeridiansAttackYinYang);
            await InitEffectCollection("Yang stimulierend", CategoryValue.MeridiansAttackYinYang);
            await InitEffectCollection("Angriff von Wind", CategoryValue.MeridiansAttackYinYang);
            await InitEffectCollection("Angriff von Kälte", CategoryValue.MeridiansAttackYinYang);
        }

        public async Task InitMolecules()
        {
            // Initialize Molecules
            await InitMoleculeCollection("α- und β-Pinen", SubstanceValue.Monoterpene);
            await InitMoleculeCollection("p-Cymen", SubstanceValue.Monoterpene);
            await InitMoleculeCollection("Myrcen", SubstanceValue.Monoterpene);
            await InitMoleculeCollection("Limonen", SubstanceValue.Monoterpene);
            await InitMoleculeCollection("Terpinolen", SubstanceValue.Monoterpene);
            await InitMoleculeCollection("Ocimen", SubstanceValue.Monoterpene);
            await InitMoleculeCollection("Sabinen", SubstanceValue.Monoterpene);
            await InitMoleculeCollection("Linalool", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Geraniol", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Vetiveron", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Menthol", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Citronellol", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("α-Terpineol", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Borneol", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Fenchol", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Nerol ", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Pinocarveol", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Thujanol", SubstanceValue.Monoterpenole);
            await InitMoleculeCollection("Bisabolen", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Cadinen", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Chamazulen", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Caryophyllen", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Germacren", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Elemen", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Santalen", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Vetiven", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Cedren", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Himachalen", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Cubeeben", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Aromadendren", SubstanceValue.Sesquiterpene);
            await InitMoleculeCollection("Patchoulol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Nerolidol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Carotol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Farnesol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Bisabolol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Cedrol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Dauctol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Italidion", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Sclareol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Incensol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Phytol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Indol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Abienol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Incensol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Zingiberol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Viridiflorol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Santalol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Vetiverol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Himacholol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Cedrol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Santalol", SubstanceValue.SesquiterpenoleDiterpenole);
            await InitMoleculeCollection("Citral", SubstanceValue.Aldehyde);
            await InitMoleculeCollection("Geranial", SubstanceValue.Aldehyde);
            await InitMoleculeCollection("Neral", SubstanceValue.Aldehyde);
            await InitMoleculeCollection("Citronellal", SubstanceValue.Aldehyde);
            await InitMoleculeCollection("Sisensal", SubstanceValue.Aldehyde);
            await InitMoleculeCollection("Santal", SubstanceValue.Aldehyde);
            await InitMoleculeCollection("Linalylacetat", SubstanceValue.Ester);
            await InitMoleculeCollection("Geranylacetat", SubstanceValue.Ester);
            await InitMoleculeCollection("Bornylacetat", SubstanceValue.Ester);
            await InitMoleculeCollection("Citronelylacetat", SubstanceValue.Ester);
            await InitMoleculeCollection("Nerylacetat", SubstanceValue.Ester);
            await InitMoleculeCollection("Phytylacetat", SubstanceValue.Ester);
            await InitMoleculeCollection("Isobutyl- und Isoamylangelat", SubstanceValue.Ester);
            await InitMoleculeCollection("Terpinylacetat", SubstanceValue.Ester);
            await InitMoleculeCollection("Methylacetat", SubstanceValue.Ester);
            await InitMoleculeCollection("Menthon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Atlanton", SubstanceValue.Ketone);
            await InitMoleculeCollection("Leptospermon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Borneon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Nootkaton", SubstanceValue.Ketone);
            await InitMoleculeCollection("Carvon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Davanon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Fenchon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Italidion", SubstanceValue.Ketone);
            await InitMoleculeCollection("Iso-Menthon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Verbenon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Iso-Pinocamphon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Pinocarvon", SubstanceValue.Ketone);
            await InitMoleculeCollection("Iron", SubstanceValue.Ketone);
            await InitMoleculeCollection("Thujon", SubstanceValue.Ketone);
            await InitMoleculeCollection("1,8-Cineol", SubstanceValue.Oxide);
            await InitMoleculeCollection("Bisabololoxid", SubstanceValue.Oxide);
            await InitMoleculeCollection("Linalooloxid", SubstanceValue.Oxide);
            await InitMoleculeCollection("Rosenoxide", SubstanceValue.Oxide);
            await InitMoleculeCollection("Eugenol", SubstanceValue.Phenole);
            await InitMoleculeCollection("Carvacrol", SubstanceValue.Phenole);
            await InitMoleculeCollection("Thymol", SubstanceValue.Phenole);
            await InitMoleculeCollection("Methyleugenol", SubstanceValue.Phenylpropanderivate);
            await InitMoleculeCollection("Methylcavicol (Estragol)", SubstanceValue.Phenylpropanderivate);
            await InitMoleculeCollection("Cis- und Trans-Anethol", SubstanceValue.Phenylpropanderivate);
            await InitMoleculeCollection("Zimtaldehyd", SubstanceValue.Phenylpropanderivate);
            await InitMoleculeCollection("Methylsalicylat", SubstanceValue.AromaticEster);
            await InitMoleculeCollection("Methylanthranilat", SubstanceValue.AromaticEster);
            await InitMoleculeCollection("Geranylformiat", SubstanceValue.AromaticEster);
            await InitMoleculeCollection("Benzylbenzoat", SubstanceValue.AromaticEster);
            await InitMoleculeCollection("Benzylacetat", SubstanceValue.AromaticEster);
            await InitMoleculeCollection("Eugenylacetat", SubstanceValue.AromaticEster);
            await InitMoleculeCollection("Angelicin", SubstanceValue.Cumarin);
            await InitMoleculeCollection("Furocumarine", SubstanceValue.Cumarin);
            await InitMoleculeCollection("Benzaldehyd", SubstanceValue.AromaticAldehyde);
            await InitMoleculeCollection("Vanillin", SubstanceValue.AromaticAldehyde);
            await InitMoleculeCollection("Phenylethylalkohol", SubstanceValue.AromaticAlcohol);
            await InitMoleculeCollection("Methylethylakohol", SubstanceValue.AromaticAlcohol);
        }

        public async Task InitUsersAndRoles()
        {
            // Initialize Roles
            await InitRoleCollection(RoleValue.Admin);
            await InitRoleCollection(RoleValue.Friend);

            // Initialize Users
            // Hash PW for: 123456
            await InitUserCollection("duftfinder@gmail.com", "20000:QMzEVU8vsURxmfyrFYOuSg==:zGYaINivZ+y98K7y6vPJUextJCE=", true, true, false, RoleValue.Admin, "Barbara", "Held", true);
        }
        public async Task InitConfigurationValues()
        {
            // Initialize configuration values
            await InitConfigurationCollection("smtpServer", "smtp.gmail.com", "Smtp Server des E-Mail Anbieters.");
            await InitConfigurationCollection("smtpPort", "587", "Smtp Port des E-Mail Anbieters.");
            await InitConfigurationCollection("smtpEnableSsl", "true", "SSL Enabling");
            await InitConfigurationCollection("smtpEmailUser", "duftfinder@gmail.com", "E-Mail-Adresse des Absenders.");
            await InitConfigurationCollection("smtpEmailUserPassword", "MtDuftfinder?2018", "Passwort des E-Mail-Accounts des Absenders (in Klartext).");
            await InitConfigurationCollection("smtpEmailFrom", "duftfinder@gmail.com", "Absendername");
            await InitConfigurationCollection("verifyAccountEmailSubject", "Ihre Registrierung bei Duftfinder", "\"Betreff-Text\" vom E-Mail, welches der User nach seiner Registration erhält.");
            await InitConfigurationCollection("verifyAccountEmailText", "Guten Tag<p>Vielen Dank f&uuml;r Ihre Registration bei Duftfinder. Um die Registration zu best&auml;tigen, klicken Sie bitte den folgenden Link: <a href={0} target=\"_blank\">{0}</a></p><p>Ihre Registration muss daraufhin von der Administratorin best&auml;tigt werden. Dies kann einige Tage dauern. Sobald Ihr Account freigeschalten wurde, werden Sie per E-Mail informiert. Ihr Account wird erst dann nutzbar sein.</p><p>Mit freundlichen Gr&uuml;ssen und bis bald auf Duftfinder</p><p>Barbara Held</p>", "E-Mail-Text vom E-Mail, welches der User nach seiner Registration erhält. {0} ist der Platzhalter für den Verifizierungs-Link, den der neue User in der E-Mail erhält und darf nicht entfernt werden.");
            await InitConfigurationCollection("infoAboutRegistrationEmailSubject", "Neuer Benutzer registriert", "\"Betreff-Text\" vom E-Mail, welches der Admin nach der Registrationsbestätigung eines neuen Users erhält.");
            await InitConfigurationCollection("infoAboutRegistrationEmailText", "Hallo Barbara<p>Soeben hat ein Benutzer mit der E-Mail-Adresse {0} seine Registration in Duftfinder verifiziert.</p><p>Geh doch bei Gelegenheit einmal in die Benutzerverwaltung von Duftfinder und pr&uuml;fe, ob du diesen Benutzer best&auml;tigen m&ouml;chtest.</p><p>Liebe Gr&uuml;sse von der Entwicklerin von Duftfinder</p>", "E-Mail-Text vom E-Mail, welches der Admin nach der Registrationsbestätigung eines neuen Users erhält. {0} ist der Platzhalter für die E-Mail-Adresse des neuen Users und darf nicht entfernt werden.");
            await InitConfigurationCollection("infoAboutRegistrationConfirmationSubject", "Registration abgeschlossen", "\"Betreff-Text\" vom E-Mail, welches der User nach der Registrationsbestätigung vom Admin erhält.");
            await InitConfigurationCollection("infoAboutRegistrationConfirmationText", "Guten Tag<p>Ihre Registration bei Duftfinder wurde soeben von der Administratorin best&auml;tigt. Sie k&ouml;nnen sich nun unter <a href={0} target=\"_blank\">{0}</a> einloggen.</p><p>Mit freundlichen Gr&uuml;ssen und bis bald auf Duftfinder</p><p>Barbara Held</p>", "E-Mail-Text vom E-Mail, welches der User nach seiner Registration erhält. {0} ist der Platzhalter für den Link, um das Passwort zu ändern und darf nicht entfernt werden.");
            await InitConfigurationCollection("forgotPasswordEmailSubject", "Passwort Reset", "\"Betreff-Text\" vom E-Mail, welches der User erhält, wenn er sein Passwort vergessen hat und zurücksetzen will.");
            await InitConfigurationCollection("forgotPasswordEmailText", "Guten Tag<p>Sie haben das &Auml;ndern Ihres Passworts bei Duftfinder beantragt. Um Ihr Passwort zu &auml;ndern, klicken Sie bitte den folgenden Link: <a href={0} target=\"_blank\">{0}</a></p><p>Ihr Passwort kann daraufhin neu gesetzt werden.</p><p>Mit freundlichen Gr&uuml;ssen und bis bald auf Duftfinder</p><p>Barbara Held</p>", "E-Mail-Text vom E-Mail, welches der User nach erhält, wenn er sein Passwort vergessen hat und zurücksetzen will. {0} ist der Platzhalter für den Verifizierungs-Link, den der User in der E-Mail erhält und darf nicht entfernt werden.");

        }

        /// <summary>
        /// Initializes Substance collection in database with default values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="substanceValue"></param>
        /// <returns></returns>
        private async Task InitSubstanceCollection(SubstanceValue substanceValue)
        {
            string substanceValueString = substanceValue.ToString();
            int sortOrder = Convert.ToInt32(substanceValue);

            IMongoCollection<Substance> substanceCollection = _mongoContext.Database.GetCollection<Substance>(nameof(Substance));

            bool isDuplicate = await MongoHelper<Substance>.IsDuplicate(Constants.Name, substanceValueString, substanceCollection, null);

            if (!isDuplicate)
            {
                Substance substance = new Substance {Name = substanceValueString, SortOrder = sortOrder};
                await substanceCollection.InsertOneAsync(substance);
                Log.Info($"Inserted substance {substanceValueString} with Id {substance.ObjectId} into database.");

                await InitIsGeneralMoleculeCollection(substanceValue, substance.ObjectId);
            }
        }

        /// <summary>
        /// Initializes Molecule collection in database with default IsGeneral values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="substanceValue"></param>
        /// <param name="substanceId"></param>
        /// <returns></returns>
        private async Task InitIsGeneralMoleculeCollection(SubstanceValue substanceValue, ObjectId substanceId)
        {
            string substanceValueString = substanceValue.ToString();

            IMongoCollection<Molecule> moleculeCollection = _mongoContext.Database.GetCollection<Molecule>(nameof(Molecule));
            bool isDuplicate = await MongoHelper<Molecule>.IsDuplicate(Constants.Name, substanceValueString, moleculeCollection, null);

            if (!isDuplicate)
            {
                // Inserts a general molecule for the substance.
                await moleculeCollection.InsertOneAsync(new Molecule { Name = substanceValueString, IsGeneral = true, SubstanceId = substanceId});
                Log.Info($"Inserted isGeneral molecule {substanceValueString} into database.");
            }
        }

        /// <summary>
        /// Initializes Category collection in database with default values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="categoryValue"></param>
        /// <returns></returns>
        private async Task InitCategoryCollection(CategoryValue categoryValue)
        {
            string categoryValueString = categoryValue.ToString();
            int sortOrder = Convert.ToInt32(categoryValue);

            IMongoCollection<Category> categoryCollection = _mongoContext.Database.GetCollection<Category>(nameof(Category));
            bool isDuplicate = await MongoHelper<Category>.IsDuplicate(Constants.Name, categoryValueString, categoryCollection, null);

            if (!isDuplicate)
            {
                await categoryCollection.InsertOneAsync(new Category { Name = categoryValueString, SortOrder = sortOrder });
                Log.Info($"Inserted category {categoryValue} into database.");
            }
        }

        /// <summary>
        /// Initializes Essential Oil collection in database with default values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="name"></param>
        /// <param name="nameLatin"></param>
        /// <param name="essentialOilType"></param>
        /// <returns></returns>
        private async Task InitEssentialOilCollection(string name, string nameLatin, EssentialOilType essentialOilType)
        {
            string essentialOilTypeString = essentialOilType.ToString();

            IMongoCollection<EssentialOil> essentialOilCollection = _mongoContext.Database.GetCollection<EssentialOil>(nameof(EssentialOil));

            bool isDuplicate = await MongoHelper<EssentialOil>.IsDuplicate(Constants.Name, name, essentialOilCollection, null);

            if (!isDuplicate)
            {
                EssentialOil essentialOil = new EssentialOil { Name = name, NameLatin = nameLatin, Type = essentialOilTypeString };
                await essentialOilCollection.InsertOneAsync(essentialOil);
                Log.Info($"Inserted essentialOil {name} with Id {essentialOil.ObjectId} into database.");
            }
        }

        /// <summary>
        /// Initializes Effect collection in database with default values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="name"></param>
        /// <param name="categoryValue"></param>
        /// <returns></returns>
        private async Task InitEffectCollection(string name, CategoryValue categoryValue)
        {
            string categoryValueString = categoryValue.ToString();

            IMongoCollection<Effect> effectCollection = _mongoContext.Database.GetCollection<Effect>(nameof(Effect));
            bool isDuplicate = await MongoHelper<Effect>.IsDuplicate(Constants.Name, name, effectCollection, null);

            if (!isDuplicate)
            {
                // Get category by name.
                IMongoCollection<Category> categoryCollection = _mongoContext.Database.GetCollection<Category>(nameof(Category));
                FilterDefinition<Category> bsonFilter = Builders<Category>.Filter.Eq(nameof(Category.Name), categoryValueString);
                Category category = await categoryCollection.Find(bsonFilter, _findOptions).SingleOrDefaultAsync();

                Effect effect = new Effect { Name = name, CategoryId = category.ObjectId};
                await effectCollection.InsertOneAsync(effect);
                Log.Info($"Inserted effect {name} with Id {effect.ObjectId} into database.");
            }
        }

        /// <summary>
        /// Initializes Molecule collection in database with default values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="name"></param>
        /// <param name="substanceValue"></param>
        /// <returns></returns>
        private async Task InitMoleculeCollection(string name, SubstanceValue substanceValue)
        {
            string substanceValueString = substanceValue.ToString();

            IMongoCollection<Molecule> moleculeCollection = _mongoContext.Database.GetCollection<Molecule>(nameof(Molecule));
            bool isDuplicate = await MongoHelper<Molecule>.IsDuplicate(Constants.Name, name, moleculeCollection, null);

            if (!isDuplicate)
            {
                // Get substance by name.
                IMongoCollection<Substance> substanceCollection = _mongoContext.Database.GetCollection<Substance>(nameof(Substance));
                FilterDefinition<Substance> bsonFilter = Builders<Substance>.Filter.Eq(nameof(Substance.Name), substanceValueString);
                Substance substance = await substanceCollection.Find(bsonFilter, _findOptions).SingleOrDefaultAsync();

                Molecule molecule = new Molecule { Name = name, SubstanceId = substance.ObjectId};
                await moleculeCollection.InsertOneAsync(molecule);
                Log.Info($"Inserted molecule {name} with Id {molecule.ObjectId} into database.");
            }
        }

        /// <summary>
        /// Initializes User collection in database with demo values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <param name="isAccountVerified"></param>
        /// <param name="isConfirmed"></param>
        /// <param name="isInactive"></param>
        /// <param name="roleValue"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="isSystemAdmin"></param>
        /// <returns></returns>
        private async Task InitUserCollection(string email, string passwordHash, bool isAccountVerified, bool isConfirmed, bool isInactive, RoleValue roleValue, string firstName, string lastName, bool isSystemAdmin)
        {
            string roleValueString = roleValue.ToString();

            IMongoCollection<User> userCollection = _mongoContext.Database.GetCollection<User>(nameof(User));
            bool isDuplicate = await MongoHelper<User>.IsDuplicate(Constants.Email, email, userCollection, null);

            if (!isDuplicate)
            {
                // Get role by name.
                IMongoCollection<Role> roleCollection = _mongoContext.Database.GetCollection<Role>(nameof(Role));
                FilterDefinition<Role> bsonFilter = Builders<Role>.Filter.Eq(nameof(Role.Name), roleValueString);
                Role role = await roleCollection.Find(bsonFilter, _findOptions).SingleOrDefaultAsync();

                User user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = passwordHash,
                    IsAccountVerified = isAccountVerified,
                    IsConfirmed = isConfirmed,
                    IsInactive = isInactive,
                    IsSystemAdmin = isSystemAdmin,
                    RoleId = role.ObjectId,
                    PasswordResetKey = null
                }; 

                await userCollection.InsertOneAsync(user);
                Log.Info($"Inserted user with email {email} with Id {user.ObjectId} into database.");
            }
        }

        /// <summary>
        /// Initializes User collection in database with demo values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="roleValue"></param>
        /// <returns></returns>
        private async Task InitRoleCollection(RoleValue roleValue)
        {
            string roleValueString = roleValue.ToString();

            IMongoCollection<Role> roleCollection = _mongoContext.Database.GetCollection<Role>(nameof(Role));
            bool isDuplicate = await MongoHelper<Role>.IsDuplicate(Constants.Name, roleValueString, roleCollection, null);

            if (!isDuplicate)
            {
                Role role = new Role { Name = roleValueString };
                await roleCollection.InsertOneAsync(role);
                Log.Info($"Inserted role with name {role} with Id {role.ObjectId} into database.");
            }
        }

        /// <summary>
        /// Initializes Configuration collection in database with default values.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private async Task InitConfigurationCollection(string key, string value, string description)
        {
            IMongoCollection<Configuration> configurationCollection = _mongoContext.Database.GetCollection<Configuration>(nameof(Configuration));

            bool isDuplicate = await MongoHelper<Configuration>.IsDuplicate(Constants.Key, key, configurationCollection, null);

            if (!isDuplicate)
            {
                Configuration configuration = new Configuration { Key = key, Value = value, Description = description};
                await configurationCollection.InsertOneAsync(configuration);
                Log.Info($"Inserted configuration {configuration} with Id {configuration.ObjectId} into database.");
            }
        }

    }
}