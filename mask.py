import tkinter as tk

#data=[4,16,128,2,64,1,8,32]
data=[1,2,4,8,16,32,64,128]
# Функция для вычисления битовой маски на основе состояния чекбоксов
def calculate_mask():
    mask = 0
    for i in range(8):
        if checkboxes[i].var.get():
            mask += data[i]  # Добавляем 2^i, если чекбокс отмечен
    result_label.config(text=f"Рассчитанная маска: {mask}")

root = tk.Tk()
root.title("Вычисление маски")

# Список для чекбоксов
checkboxes = []

# Создаем 8 чекбоксов и размещаем их в сетке
for i in range(8):
    var = tk.IntVar()  # Переменная для хранения состояния чекбокса
    checkbox = tk.Checkbutton(root, text=f"Чекбокс {data[i]}", variable=var, command=calculate_mask)
    checkbox.var = var  # Сохраняем ссылку на переменную для использования в calculate_mask
    checkboxes.append(checkbox)

# Размещаем чекбоксы в сетке 3x3 (с пропуском для 5-го чекбокса)
checkboxes[0].grid(row=0, column=0, padx=10, pady=5)
checkboxes[1].grid(row=0, column=1, padx=10, pady=5)
checkboxes[2].grid(row=0, column=2, padx=10, pady=5)

checkboxes[3].grid(row=1, column=0, padx=10, pady=5)
checkboxes[4].grid(row=1, column=2, padx=10, pady=5)

checkboxes[5].grid(row=2, column=0, padx=10, pady=5)
checkboxes[6].grid(row=2, column=1, padx=10, pady=5)
checkboxes[7].grid(row=2, column=2, padx=10, pady=5)

# Метка для отображения результата
result_label = tk.Label(root, text="Рассчитанная маска: 0")
result_label.grid(row=3, column=0, columnspan=3, padx=10, pady=10)

# Запускаем главный цикл приложения
root.mainloop()