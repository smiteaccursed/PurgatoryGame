EXTERNAL enterBossFight()

-> main

=== main ===
Ты готов покинуть чистилище?
    + [Да]
        -> fight
    + [Нет]
        -> exit

=== fight  ===
Сегодня я присоединюсь к охоте. Я ИДУ!
~enterBossFight()
-> END

===exit===
Ну ладно. Может в другой раз... 
->END