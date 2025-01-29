import pygame

# Initialize Pygame
pygame.init()

# Screen settings
screen = pygame.display.set_mode((800, 600))
pygame.display.set_caption("Sprite Change Example")
clock = pygame.time.Clock()

# Colors
WHITE = (255, 255, 255)

# Load sprite images
sprite_idle = pygame.image.load("sprite_idle.png")  # Default sprite
sprite_action1 = pygame.image.load("sprite_action1.png")  # Sprite for key 1
sprite_action2 = pygame.image.load("sprite_action2.png")  # Sprite for key 2

# Scale the images (optional)
sprite_idle = pygame.transform.scale(sprite_idle, (100, 100))
sprite_action1 = pygame.transform.scale(sprite_action1, (100, 100))
sprite_action2 = pygame.transform.scale(sprite_action2, (100, 100))

# Sprite settings
current_sprite = sprite_idle  # Start with the idle sprite
sprite_x, sprite_y = 350, 250  # Sprite position

# Game loop
running = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:  # Quit the game
            running = False

    # Key handling
    keys = pygame.key.get_pressed()
    if keys[pygame.K_1]:  # Press 1 to change the sprite
        current_sprite = sprite_action1
    elif keys[pygame.K_2]:  # Press 2 to change the sprite
        current_sprite = sprite_action2
    else:  # No key pressed, return to idle
        current_sprite = sprite_idle

    # Drawing
    screen.fill(WHITE)  # Clear the screen
    screen.blit(current_sprite, (sprite_x, sprite_y))  # Draw the sprite

    pygame.display.flip()  # Update the display
    clock.tick(60)  # Limit FPS to 60

# Quit Pygame
pygame.quit()