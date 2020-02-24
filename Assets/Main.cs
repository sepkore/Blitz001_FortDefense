using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

public class Main : MonoBehaviour
{
    //OPTIMIZATIONS
    //Store reference instead of using "Camera.main" as it just uses Object.Find() which is costly
    //Use inline functions to reduce overhead of calls; declare before function (.NET 4.6+):
    //  [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    //Split and group canvas into sub-canvases to prevent re-drawing entire canvas for small changes
    //Turn off Raycast Target on non-interactive UI elements
    //Ensure Render Camera is set in canvases to prevent unity searching for it
    //Mark GameObjects as static as necessary; allows static batching and other improvements
    //Object pooling: create, retrieve, and return objects to hidden "pool" instead of creating and destroying; increases load time
    //      Dictionary of Pools (Queue)
    //Set Application.targetFrameRate to reduce power usage; 60 or 30
    //Avoid using GetComponent<>() and similar; instead ref/link it in editor
    //Set audio clip Load Type to "compressed in memory", Vorbis, and adjust quality for songs and ambient tracks; otherwise Decompress on Load/PCM for short sound effects
    //Use TextMesh Pro instead of Text
    //Use Atlases / Texture packing
    //Use Distance/camera culling and Occulusion culling
    //Enable GPU Instancing on Material of repeated GameObjects (enemies)
    //Bake High Poly Meshes to Normal Map to apply to Low Poly Meshes (Blender)
    //Use multiple primitive colliders instead of Mesh Collider where possible; small boost
    //Use Profiler.BeginSample("sample1") / Profiler.EndSample(); to profile segments of code in scripts


    const int FRAMERATE = 40;


    public static Main main;

    public LogicUI ui;

    public Turret player;
    public Turret turret02;
    public Turret turret03;
    public Fort fort;
    public GameObject ground;
    
    public Enemy enemy01; //TODO: create List or array for enemy prefabs
    public Enemy enemy02;
    List<Enemy> enemies; //current enemies in scene
    public Dictionary<string, Enemy> enemy_prefabs = new Dictionary<string, Enemy>(); //set in editor
    public Dictionary<string, Enemy> enemy_clones = new Dictionary<string, Enemy>();
    public int enemy_count = 0; //enemy count in scene; used for clone ids primarily

    public float world_ceiling = 1000.0f;

    public float cam_speed = 5.0f;
    public float projectile_speed = 50.0f;

    public GameObject projectile;
    Vector3 projectile_offset = new Vector3(0.25f, 0.0f, 0.0f);

    public int wave_number = 1;

    public Wave wave_current;
    public bool isWaveStarted = false;
    public int wave_kills = 0;

    public bool isPaused = false;
    
    Color fort_original_color;


    //Audio
    public AudioMixer audio_mixer;

    Dictionary<string,AudioSource> audio_bgm_map = new Dictionary<string,AudioSource>();
    AudioSource audio_bgm_current;
    //public AudioSource audio_bgm_mainmenu;
    public AudioSource audio_bgm_music01;
    public AudioSource audio_bgm_music02;

    Dictionary<string,AudioSource> audio_sfx_map = new Dictionary<string,AudioSource>();
    public AudioSource audio_sfx_hit01;


    //VFX
    public GameObject vfx_basic_attack;
    public GameObject vfx_basic_impact;
    


    //TODO: Create GameData struct to pass to SaveData.addGameData(GameData)
    //TODO: Create array of Enemies in scene to pass to SaveData.addEnemyData()


    void saveGame()
    {
        //TODO: Create and populate SaveData
        SaveData data = new SaveData();
        //TODO: Add game state data
        //TODO: Add upgrades and abilities
        data.addPlayerData(player);
        data.addFortData(fort);
        //TODO: Add enemies

        Persistor.save(data);
    }

    void loadGame()
    {

    }


    void initEnemies()
    {
        //TODO: allow adding in editor

        enemy_prefabs.Add("enemy01", enemy01);
        enemy_prefabs.Add("enemy02", enemy02);
    }

    void initAssets()
    {
        this.fort_original_color = this.fort.transform.Find("Mesh").GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
    }

    void Awake()
    {
        Application.targetFrameRate = Main.FRAMERATE;

        if(main == null){
            DontDestroyOnLoad(gameObject);
            main = this;
        }
        else if(main != this){
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("Main started.");

        this.enemies = new List<Enemy>();
        initEnemies();
        initAssets();

        //create audio bgm and sound maps
        this.audio_bgm_map.Add("music01", this.audio_bgm_music01);
        this.audio_bgm_map.Add("music02", this.audio_bgm_music02);

        this.audio_sfx_map.Add("hit01", this.audio_sfx_hit01);

        
        //start audio
        audioPlayMusic("music01");
        //this.audio_bgm_current = this.audio_bgm_map["music01"];
        //this.audio_bgm_current.enabled = true;
        //this.audio_bgm_current.Play();


        //TODO: if any saved data, load it, otherwise start new game
        //TODO: call newGame/gameStart from Scene's Logic; This Main doesn't need to auto start
        ui.pause();
        //onNewGame(); //only enabled when debugging game without main menu; called from "Play"
        
    }

    void Update()
    {
        //Check fort HP; end game if <= 0
        if(this.fort.hp <= 0){
            ui.pause();
            ui.openGameOver();
        }

        //Tick through wave, wait for delay to spawn next     
        if(wave_current != null){   
            if(this.isWaveStarted){ //TODO: isWaveStarted redundant currently. //if wave started and no enemies left, end wave - go to summary/upgrades
                if(this.wave_kills >= this.wave_current.enemy_count){
                    this.wave_kills = 0;
                    ui.openSummary();
                }
            }

            wave_current.timer += Time.deltaTime;
            //Debug.Log("Checking Current Wave timer: " + wave_current.timer.ToString());
            //Debug.Log("..Current Wave turn: " + wave_current.turn.ToString());
            //Debug.Log("..Current Wave turn delay: " + wave_current.delays[wave_current.turn].ToString());
            if(wave_current.hasEnemiesLeft()){
                if(wave_current.timer >= wave_current.delays[wave_current.turn]){
                    string enemy_id = wave_current.getNextEnemy();
                    Enemy nme = spawnEnemy(enemy_id, randomEnemyPos());
                    enemies.Add(nme);
                }
            }
            
        }


        pollInput();
        
        //Check enemies isAlive
        for(int i=this.enemies.Count-1; i>=0; i--){
            if(!this.enemies[i].isAlive){ //enemy dead; remove, reward, etc
                //TODO: Reward
                this.reward(enemies[i]);
                this.wave_kills++;

                removeFromProximities(this.enemies[i].gameObject); //remove from turret lists
                Destroy(this.enemies[i].gameObject, 1.0f); //destroy after 1 second
                this.enemies[i].rb.velocity = new Vector3(0.0f, 0.0f, 0.0f); //stop motion
                this.enemies[i].transform.Find("Sprite").gameObject.GetComponent<Collider>().enabled = false; //disable collision
                this.enemies.RemoveAt(i);
            }
        }


        //Turrets scan, determine target, fire on target 
        updateTurret(player);
        if(turret02 != null){
            updateTurret(turret02);
        }
        if(turret03 != null){
            updateTurret(turret03);
        }


        //Check if enemies are within proximity to fort; if true: attack fort at attack_rate
        for(int i=0; i<this.enemies.Count; i++){
            Enemy nme = this.enemies[i];
            nme.attack_timer += Time.deltaTime;
            float dist = Vector3.Distance(nme.transform.position, this.fort.transform.position);
            //Debug.Log(nme.clone_id + " dist: " + dist);
            if(dist <= nme.attack_range){
                if(nme.attack_timer >= nme.attack_rate){
                    attackFort(nme.attack_damage);
                    nme.attack_timer = 0.0f;
                }
            }
        }
 
        updateUI();
    }

    private void updateTurret(Turret turret)
    {
        //Update timer
        turret.attack_timer += Time.deltaTime;

        //Get target
        turret.target = null;
        turret.scanForTargets();
        if(turret.targets.Count > 0){
            turret.selectClosestTarget();
        }

        //Fire on target if has target
        if(turret.target != null){
            basicAttackTarget(turret);
        }
        
    }

    private void removeFromProximities(GameObject obj)
    {
        player.removeProximityEnemy(obj);
        if(turret02 != null){ turret02.removeProximityEnemy(obj); }
        if(turret03 != null){ turret03.removeProximityEnemy(obj); }
    }

    private void clearTurrets()
    {
        player.clear();
        if(turret02 != null){ turret02.clear(); }
        if(turret03 != null){ turret03.clear(); }
    }

    private void reward(Enemy nme)
    {
        this.player.gold += nme.gold;
    }

    private void FixedUpdate()
    {
        //Update enemy movement    
        foreach(Enemy nme in enemies){
            nme.rb.velocity = getTrajectoryEnemy(nme).normalized * nme.speed;

            Vector3 movement = getTrajectoryEnemy(nme).normalized * nme.speed * Time.fixedDeltaTime;
            nme.rb.MovePosition(nme.rb.position + movement); //move manually
            this.moveToGround(nme.gameObject); //pin to ground to navigate terrain
        }
    }

    void updateUI()
    {
        ui.hpbar_fort.value_current = this.fort.hp;
        ui.hpbar_fort.value_max = this.fort.hp_max;
        ui.hpbar_fort.text_display.text = this.fort.hp.ToString();

        ui.text_wave_number.GetComponent<Text>().text = "Wave " + this.wave_number.ToString();
        ui.text_game_gold.GetComponent<Text>().text = "Gold: " + this.player.gold.ToString();
        ui.text_upgrades_gold.GetComponent<Text>().text = "Gold: " + this.player.gold.ToString();
    }



    void pollInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            //UnityEditor.EditorApplication.isPlaying = false; //Remove when deploying
            Application.Quit();
        }


        if(Input.GetAxis("Mouse ScrollWheel") > 0){
            Camera.main.transform.Translate(Camera.main.transform.forward * cam_speed * Time.deltaTime, Space.World);
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0){
            Camera.main.transform.Translate(Camera.main.transform.forward * -cam_speed * Time.deltaTime, Space.World);
        }

        if(Input.GetKey(KeyCode.W)){
            if(Input.GetKey(KeyCode.LeftShift)){     
            }
            else{  
            }
        }
        if(Input.GetKey(KeyCode.A)){
            if(Input.GetKey(KeyCode.LeftShift)){
            }
            else{
            }
        }
        if(Input.GetKey(KeyCode.S)){
            if(Input.GetKey(KeyCode.LeftShift)){
            }
            else{
            }
        }
        if(Input.GetKey(KeyCode.D)){
            if(Input.GetKey(KeyCode.LeftShift)){
            }
            else{
            }
        }
        if(Input.GetKey(KeyCode.LeftShift)){
        }
        if(Input.GetKey(KeyCode.LeftControl)){
        }
        
        if(Input.GetKeyDown(KeyCode.Space)){

        }


        if(Input.GetKeyDown(KeyCode.F2)){
            //SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
        }
        if(Input.GetKeyDown(KeyCode.F1)){
            //SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); //or Additive
        }


        if(Input.GetKeyDown(KeyCode.F5)){
        }
        if(Input.GetKeyDown(KeyCode.F9)){
        }


        if(Input.GetKeyDown(KeyCode.Alpha1)){
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
        }


        //if(Input.GetMouseButtonDown(0)){ //Left click //android considers touch a left click?
        //    Debug.Log("Left Clicked");
        //    spawnProjectile(this.player.transform.position + this.projectile_offset, getTrajectory(Input.mousePosition));
        //}

        if(Input.GetMouseButton(0)){ //Left mouse held down
            //this.basicAttack();
        }

        /*
        for(int i=0; i<Input.touchCount; i++){
            if(Input.GetTouch(i).phase == TouchPhase.Began){
                Debug.Log("Touched");
                spawnProjectile(this.player.transform.position + this.projectile_offset, getTrajectory(Input.GetTouch(i).position));
            }
        }
        */

        if(Input.GetMouseButtonDown(1)){ //right click
            Debug.Log("Right Clicked");
        }
        if(Input.GetMouseButtonDown(2)){ //middle click
            Debug.Log("Middle Clicked");
        }



    }


    

    public void basicAttackTarget(Turret turret)
    {
        //Attack turret's current target
        if(turret.attack_timer >= turret.attack_rate){
            spawnVFX(this.vfx_basic_attack, turret.transform.position);
            spawnProjectile(turret, getTrajectory(turret.transform.position, turret.target.transform.position));
            turret.attack_timer = 0.0f; 
        }    
    }


    void spawnProjectile(Turret source, Vector3 trajectory)
    {
        Vector3 zero_vector = new Vector3(0, 0, 0);
        if(trajectory == zero_vector){ return; }

        Quaternion rot = source.transform.rotation;
        GameObject clone_projectile = Instantiate(this.projectile, source.transform.position, rot);
        clone_projectile.GetComponent<Projectile>().source = source;
        clone_projectile.GetComponent<Projectile>().lifetime = 1.0f;


        //Set velocity to speed along vector to target
        clone_projectile.GetComponent<Rigidbody>().velocity = trajectory * clone_projectile.GetComponent<Projectile>().speed;
    }

    void spawnVFX(GameObject vfx_prefab, Vector3 pos)
    {
        GameObject clone_vfx = Instantiate(vfx_prefab, pos, vfx_prefab.transform.rotation);
    }

    Vector3 getTrajectoryFromInput(Vector3 input_pos_screen)
    {
        Vector3 trajectory = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(input_pos_screen);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            trajectory = hit.point - this.player.transform.position;
            //Debug.Log("Input POS World: " + hit.point);
        }   
        return trajectory.normalized;
    }

    Vector3 getTrajectory(Vector3 pos_source, Vector3 pos_target) //get trajectory from turret to target
    {
        Vector3 trajectory = new Vector3();
        trajectory = pos_target - pos_source;
        return trajectory.normalized;
    }

    Vector3 getTrajectoryEnemy(Enemy nme)
    {
        nme.trajectory = this.fort.transform.position - nme.transform.position;
        return nme.trajectory;
    }


    public void onNewGame()
    {
        //TODO: Init, Add to scene, and Position player, fort, and other global objects
        //Vector3 ppos = new Vector3(-7.9444f, 1.277f, -0.127931f);
        //this.player.transform.position = ppos;
        //this.player.gameObject.SetActive(true);

        //Vector3 fpos = new Vector3(-9.14f, 0.46f, 0.13f);
        //this.fort.transform.position = fpos;
        //this.fort.gameObject.SetActive(true);

        //TODO: Generate Wave, Start Wave
        //TODO: Generate Wave: Clone and modify enemies to List, scale enemies to wave_level, create and set enemies to "wave timeline" (spawn delay, groupings), set enemy count
        //TODO: Start Wave: Execute timers, bring enemies into scene, set enemies into motion

        Debug.Log("Starting New Game");
        audioPlayMusic("music02");
        this.wave_kills = 0;
        resetPlayer();
        generateWave();
        
    }


    Enemy spawnEnemy(string enemy_id, Vector3 pos)
    {
        //Debug.Log("Spawning enemy: " + enemy_id);
        Enemy prefab_enemy = this.enemy_prefabs[enemy_id];
        Enemy clone_enemy = Instantiate(prefab_enemy, pos, prefab_enemy.transform.rotation);
        clone_enemy.clone_id = enemy_id + "-" + this.enemy_count;
        this.enemy_clones.Add(clone_enemy.clone_id, clone_enemy);
        this.enemy_count++;
        return clone_enemy;
    }

    Vector3 randomEnemyPos()
    {
        float pos_x = this.fort.transform.position.x + Random.Range(32.0f, 48.0f);
        float pos_y = 200.0f; //overridden in groundHeight()
        float pos_z = this.fort.transform.position.z + Random.Range(-8.0f, 8.0f);
        Vector3 pos = new Vector3(pos_x, pos_y, pos_z);
        pos.y = groundHeight(pos);
        return pos;
    }

    public float groundHeight(Vector3 pos)
    {
        //Get ground mesh collider to raycast against; cast ray downward from ceiling at x,z of object
        //Collider ground_collider = this.ground.transform.Find("Mesh").GetComponent<Collider>();
        Collider ground_collider = this.ground.GetComponentsInChildren<Collider>()[0];
        pos.y = this.world_ceiling;
        Ray ray = new Ray(pos, -Vector3.up);
        RaycastHit hit;
        ground_collider.Raycast(ray, out hit, this.world_ceiling * 2.0f);
        return hit.point.y;
    }

    public void moveToGround(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        pos.y = groundHeight(pos);
        obj.transform.position = pos;
    }


    public void generateWave()
    {
        Debug.Log("Generating Wave");

        resetPlayer(); //TODO: call from wherever generateWave is called from
        
        Wave wave = new Wave();

        int ecount = 30 * this.wave_number; //TEMP: Increase with wave level
        for(int i=0; i<ecount; i++){
            int rndi = Random.Range(1,101);
            //Debug.Log("Random Int: " + rndi);
            string enemy_id = "";
            if(rndi <= 70){ enemy_id = "enemy01"; }
            if(rndi > 70){ enemy_id = "enemy02"; }
            wave.addEnemy(enemy_id, Random.Range(0.1f, 1.5f));
        }

        this.wave_current = wave;
        this.isWaveStarted = true;
    }

    public void resetPlayer()
    {
        //Reset fort, abilities, etc //TODO: move to separate function
        this.fort.hp = this.fort.hp_max;

        this.clearTurrets();
    }

    public void onHitEnemy(Turret source, Enemy nme, Vector3 hitpos)
    {
        //Enemy nme = this.enemy_clones[enemy_id];
        //nme.hit_count++;
        //Debug.Log("Enemy Hit x" + nme.hit_count + ": " + enemy_id);
  
        this.applyDamage(source, nme);

        //VFX
        spawnVFX(vfx_basic_impact, hitpos);
        
    }

    public void applyDamage(Turret source, Enemy nme)
    {
        nme.hp -= source.damage;
    }


    public void attackFort(int damage)
    {
        //Debug.Log("Fort Attacked for " + damage + " damage!");
        this.fort.hp -= damage;
        StartCoroutine(flashMesh(this.fort.gameObject));

    }

    IEnumerator flashMesh(GameObject obj)
    {
        //Debug.Log("Flashing Mesh");
        GameObject mesh = obj.transform.Find("Mesh").gameObject;
        MeshRenderer rnd = mesh.GetComponent<MeshRenderer>();
        
        rnd.material.SetColor("_BaseColor", Color.red);
        yield return new WaitForSecondsRealtime(0.05f);
        rnd.material.SetColor("_BaseColor", this.fort_original_color);
    }
    

    public void onHitGround(Vector3 hitpos)
    {
        //VFX
        Instantiate(vfx_basic_impact, hitpos, vfx_basic_impact.transform.rotation);
    }
    

    


    public void clearGame()
    {
        for(int i=this.enemies.Count-1; i>=0; i--){
            Destroy(this.enemies[i].gameObject);
        }
        this.enemies.Clear();
        this.wave_current = null;
        this.wave_number = 1;
        this.player.gold = 0;
        this.player.damage = this.player.damage_base;
        this.player.attack_rate = this.player.attack_rate_base;
        this.fort.hp_max = this.fort.hp_base;
    }


    public void audioPlayMusic(string audio_id)
    {
        if(this.audio_bgm_current != null){ this.audio_bgm_current.Stop(); }
        this.audio_bgm_current = this.audio_bgm_map[audio_id];
        this.audio_bgm_current.Play();
    }

    public void audioPlaySound(string audio_id)
    {
        this.audio_sfx_map[audio_id].Play();
    }

    public void onToggle_BGM(bool on)
    {
        GameObject clicker = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Toggle toggle = clicker.GetComponent<Toggle>();
        //AudioSource audio = this.audio_bgm.GetComponent<AudioSource>();
        if(toggle.isOn){
            this.audio_mixer.SetFloat("bgm_vol", 0.0f);
        }
        else{
            this.audio_mixer.SetFloat("bgm_vol", -80.0f);
        }
    }




    

    public void upgrade(UpgradeItem upitem)
    {
        if(upitem.rank >= upitem.rank_max){
            //TODO: Can't improve anymore!
            return;
        }

        if(this.player.gold < upitem.cost){
            //TODO: Not enough gold!
            return;
        }
        else{
            this.player.gold -= upitem.cost;
        }


        if(upitem.upgrade_id == "fort_hp"){
            this.fort.hp_max += 50;
            
        }

        if(upitem.upgrade_id == "player_speed"){
            this.player.attack_rate /= 2.0f; 
        }

        upitem.rank++;
    }

}
